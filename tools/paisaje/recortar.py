"""
Recorta de las pinturas de Goya los arboles del camino (La cometa, La nevada, La gallina ciega)
y los guarda con transparencia en Assets/Resources/Paisaje.
Uso:  python tools/paisaje/recortar.py      (necesita numpy, opencv-python y Pillow)
"""
import os, numpy as np, cv2
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..", "..")
OBRAS = os.path.join(ROOT, "Assets", "Obras")
OUT = os.path.join(ROOT, "Assets", "Resources", "Paisaje")

def load(name, box):
    im = cv2.imread(os.path.join(OBRAS, name))
    x0, y0, x1, y1 = box
    return im[y0:y1, x0:x1].copy()

def rembg_mask(bgr):
    from rembg import remove, new_session
    out = remove(Image.fromarray(cv2.cvtColor(bgr, cv2.COLOR_BGR2RGB)), session=new_session("u2net"))
    return np.array(out)[:, :, 3]

def poly(shape, pts):
    m = np.zeros(shape[:2], np.uint8)
    cv2.fillPoly(m, [np.array(pts, np.int32)], 255)
    return m

def finish(bgr, mask, name, soft=2.0, fade_bottom=0):
    mask = cv2.GaussianBlur(mask, (0, 0), soft)
    if fade_bottom:
        h = mask.shape[0]
        ramp = np.ones(h, np.float32); ramp[-fade_bottom:] = np.linspace(1, 0.6, fade_bottom)
        mask = (mask * ramp[:, None]).astype(np.uint8)
    ys, xs = np.where(mask > 8)
    y0, y1, x0, x1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
    rgba = cv2.cvtColor(bgr, cv2.COLOR_BGR2BGRA); rgba[:, :, 3] = mask
    cv2.imwrite(os.path.join(OUT, name + ".png"), rgba[y0:y1, x0:x1])
    print(name, x1 - x0, y1 - y0)

def arbol_cometa():
    """El arbol de La cometa: copa por color (verde contra el cielo) y tronco a mano."""
    im = load("02 La cometa.jpg", (540, 0, 1024, 860))
    lab = cv2.cvtColor(im, cv2.COLOR_BGR2LAB).astype(np.int16)
    L, a, b = lab[..., 0], lab[..., 1] - 128, lab[..., 2] - 128
    leaves = ((a < -4) & (b > 6) & (L < 210)) | (L < 90)   # verdes oliva y sombras (el cielo es azulado: b < 0)
    leaves = leaves.astype(np.uint8) * 255
    leaves = cv2.morphologyEx(leaves, cv2.MORPH_OPEN, np.ones((3, 3), np.uint8))
    leaves = cv2.morphologyEx(leaves, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (9, 9)))
    # tronco y rama principal (el tronco es claro, como el cielo: va a mano)
    trunk = np.zeros(im.shape[:2], np.uint8)
    spine = [(238, 80, 12), (252, 200, 14), (272, 300, 17), (330, 400, 20), (368, 500, 24), (390, 600, 28), (405, 700, 32), (420, 800, 38), (425, 860, 42)]
    for (x0, y0, w0), (x1, y1, w1) in zip(spine, spine[1:]):
        cv2.fillPoly(trunk, [np.array([(x0 - w0, y0), (x0 + w0, y0), (x1 + w1, y1), (x1 - w1, y1)], np.int32)], 255)
    cv2.line(trunk, (262, 335), (0, 180), 255, 16)        # rama larga a la izquierda
    keep = poly(im.shape, [(0, 0), (484, 0), (484, 860), (360, 860), (300, 640), (200, 480), (0, 480)])
    people = poly(im.shape, [(270, 630), (390, 630), (390, 800), (270, 800)])
    mask = cv2.bitwise_and(cv2.max(leaves, trunk), keep)
    mask[people > 0] = trunk[people > 0]
    # quedarse con lo conectado al arbol
    n, lbl, st, _ = cv2.connectedComponentsWithStats((mask > 127).astype(np.uint8))
    main = lbl[800, 420]
    mask = np.where((lbl == main) | (st[lbl, cv2.CC_STAT_AREA] > 1500), mask, 0).astype(np.uint8)
    mask[790:] = 0   # sin el pedazo de suelo de la pintura: el pie lo pone el pasto del camino
    finish(im, mask, "arbol_cometa", 1.5, 40)

def arbol_nevada():
    """El arbol doblado por el viento de La nevada: ramas por contraste con el cielo y tronco a mano."""
    im=cv2.imread(os.path.join(OBRAS,'05 La nevada.jpg'))
    H,W=im.shape[:2]
    L=cv2.cvtColor(im,cv2.COLOR_BGR2LAB)[...,0].astype(np.float32)
    d=cv2.GaussianBlur(L,(0,0),18)-L
    m=((d>7.5)*255).astype(np.uint8)
    m=cv2.morphologyEx(m,cv2.MORPH_OPEN,np.ones((2,2),np.uint8))
    def poly2(pts):
        k=np.zeros((H,W),np.uint8); cv2.fillPoly(k,[np.array(pts,np.int32)],255); return k
    # zona del arbol: copa arriba (y<720) y el corredor del tronco abajo
    keep=cv2.max(poly2([(0,150),(900,150),(900,700),(420,700),(300,760),(250,1000),(140,1440),(0,1440)]),
                 poly2([(120,700),(420,700),(300,800),(230,1000),(160,1440),(0,1440),(0,1000)]))
    people=cv2.max(poly2([(585,700),(900,700),(900,1500),(585,1500)]), poly2([(240,960),(330,920),(500,980),(520,1500),(215,1500),(190,1250)]))
    m=cv2.bitwise_and(m,keep); m[people>0]=0
    trunk=np.zeros((H,W),np.uint8)
    spine=[(30,1440,40),(90,1290,34),(150,1140,28),(210,975,22),(262,840,17),(322,705,13),(375,615,10),(450,510,7)]
    for (x0,y0,w0),(x1,y1,w1) in zip(spine,spine[1:]):
        cv2.fillPoly(trunk,[np.array([(x0-w0,y0),(x0+w0,y0),(x1+w1,y1),(x1-w1,y1)],np.int32)],255)
    mask=cv2.max(m,trunk)
    mask=cv2.morphologyEx(mask,cv2.MORPH_CLOSE,cv2.getStructuringElement(cv2.MORPH_ELLIPSE,(5,5)))
    n,lbl,st,_=cv2.connectedComponentsWithStats((mask>127).astype(np.uint8))
    main=lbl[1140,150]
    mask=np.where(lbl==main,mask,0).astype(np.uint8)
    mask[1420:]=0
    finish(im,mask,'arbol_nevada',1.2,40)

def arbol_gallina():
    """El tronco de La gallina ciega (a mano) con sus hojas; arriba esta cortado por el marco y se esfuma."""
    im=cv2.imread(os.path.join(OBRAS,'01 La gallina ciega.jpg'))
    H,W=im.shape[:2]
    def poly2(pts):
        k=np.zeros((H,W),np.uint8); cv2.fillPoly(k,[np.array(pts,np.int32)],255); return k
    lab=cv2.cvtColor(im,cv2.COLOR_BGR2LAB).astype(np.float32)
    L=lab[...,0]; a=lab[...,1]-128; b=lab[...,2]-128
    d=cv2.GaussianBlur(L,(0,0),12)-L
    leaves=(((a<0)&(d>6))|(d>14)).astype(np.uint8)*255
    leaves=cv2.bitwise_and(leaves,poly2([(700,0),(1024,0),(1024,170),(700,170)]))
    trunk=cv2.max(poly2([(961,0),(996,0),(980,60),(974,100),(974,150),(984,200),(1000,240),(1024,250),(1024,280),(1016,300),(1014,350),(1009,400),(1001,450),(991,500),(984,550),(979,600),(978,625),(865,625),(874,600),(898,550),(918,500),(931,450),(940,400),(946,350),(951,300),(945,250),(936,200),(931,150),(931,100),(950,40)]),
                 poly2([(1006,0),(1024,0),(1024,250),(1006,240),(1004,120)]))
    branch=np.zeros((H,W),np.uint8)
    cv2.polylines(branch,[np.array([(935,100),(890,65),(850,35),(800,12),(760,0)],np.int32)],False,255,14)
    mask=cv2.max(cv2.max(leaves,trunk),branch)
    mask=cv2.morphologyEx(mask,cv2.MORPH_CLOSE,cv2.getStructuringElement(cv2.MORPH_ELLIPSE,(7,7)))
    n,lbl,st,_=cv2.connectedComponentsWithStats((mask>127).astype(np.uint8))
    mask=np.where(lbl==lbl[500,950],mask,0).astype(np.uint8)
    # arriba esta cortado por el marco: se esfuma
    ramp=np.clip(np.arange(H)/110.0,0,1)**1.5
    mask=(mask*ramp[:,None]).astype(np.uint8)
    finish(im[:, :],mask,'arbol_gallina',1.5,30)

if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    arbol_cometa(); arbol_nevada(); arbol_gallina()
