#include "pch-cpp.hpp"





template <typename T1, typename T2, typename T3>
struct InvokerActionInvoker3;
template <typename T1, typename T2, typename T3>
struct InvokerActionInvoker3<T1*, T2, T3*>
{
	static inline void Invoke (Il2CppMethodPointer methodPtr, const RuntimeMethod* method, void* obj, T1* p1, T2 p2, T3* p3)
	{
		void* params[3] = { p1, &p2, p3 };
		method->invoker_method(methodPtr, method, obj, params, params[2]);
	}
};

struct ITypeConstructor_1_tCC196E183FB5A7CF798C682F9C3B70CDDA50CFFF;
struct ITypeConstructor_1_tADCE5163310B602EB1878CA0050295BFB0F6B542;
struct ITypeConstructor_1_t61D5D3D9EDD28391B0FE316A5641A34D7FF16FD0;
struct ITypeConstructor_1_t5B814DBA70C191660AF27E38032998417073D7A2;
struct ITypeConstructor_1_t63AA3F84ED218C4C0589371B74758F62E3EFCF85;
struct ITypeConstructor_1_tE8F6FB08714C8CB10D2287D360560F082E591BF9;
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
struct Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3;
struct EntityId_t982FBD037EAC5CA077B1602A7EA40E3523AA0FC8;
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C;
struct String_t;
struct UInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF;
struct UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C;
struct UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2;
struct UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C;
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
struct ITypeConstructor_tAEA6BC4B87F0F3739E4457F21908EF55F666EB08;
struct TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F;

IL2CPP_EXTERN_C RuntimeClass* StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F  : public RuntimeObject
{
	RuntimeObject* ___TypeConstructor;
};
struct AlignOfHelper_1_tAF795AF837E3C37401B92C7483D74A4E5145DB4D 
{
	uint8_t ___dummy;
	uint8_t ___data;
};
struct AlignOfHelper_1_t676424B8757032427D7761BC732079DC7512D8AE 
{
	uint8_t ___dummy;
	int32_t ___data;
};
struct AlignOfHelper_1_tE21040F72E90527C319FF9B07B398E23DF95BE2E 
{
	uint8_t ___dummy;
	uint64_t ___data;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	bool ___m_value;
};
struct Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3 
{
	uint8_t ___m_value;
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2  : public ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_pinvoke
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_com
{
};
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C 
{
	int32_t ___m_value;
};
struct Int64_t092CFB123BE63C28ACDAF65C68F21A526050DBA3 
{
	int64_t ___m_value;
};
struct IntPtr_t 
{
	void* ___m_value;
};
struct UInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF 
{
	uint64_t ___m_value;
};
struct UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926 
{
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			uint8_t* ___values;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint8_t* ___values_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___keys_OffsetPadding[8];
			uint8_t* ___keys;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___keys_OffsetPadding_forAlignmentOnly[8];
			uint8_t* ___keys_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___next_OffsetPadding[16];
			uint8_t* ___next;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___next_OffsetPadding_forAlignmentOnly[16];
			uint8_t* ___next_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___buckets_OffsetPadding[24];
			uint8_t* ___buckets;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___buckets_OffsetPadding_forAlignmentOnly[24];
			uint8_t* ___buckets_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___keyCapacity_OffsetPadding[32];
			int32_t ___keyCapacity;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___keyCapacity_OffsetPadding_forAlignmentOnly[32];
			int32_t ___keyCapacity_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___bucketCapacityMask_OffsetPadding[36];
			int32_t ___bucketCapacityMask;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___bucketCapacityMask_OffsetPadding_forAlignmentOnly[36];
			int32_t ___bucketCapacityMask_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___allocatedIndexLength_OffsetPadding[40];
			int32_t ___allocatedIndexLength;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___allocatedIndexLength_OffsetPadding_forAlignmentOnly[40];
			int32_t ___allocatedIndexLength_forAlignmentOnly;
		};
	};
};
struct UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C 
{
	UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C* ___m_NextBlock;
	int32_t ___m_NumItems;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
struct AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 
{
	uint16_t ___Index;
	uint16_t ___Version;
};
struct Union_t651696C44E98CE15C53FE2628FA73E4B102D6C21 
{
	int64_t ___m_long;
};
struct Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6 
{
	uint8_t* ___Ptr;
	int32_t ___Size;
	int32_t ___Offset;
};
struct U3CDataU3Ee__FixedBuffer_t60155FEDF2B8430C4B466CB3CDD85327C03F1116 
{
	union
	{
		struct
		{
			uint8_t ___FixedElementField;
		};
		uint8_t U3CDataU3Ee__FixedBuffer_t60155FEDF2B8430C4B466CB3CDD85327C03F1116__padding[1];
	};
};
struct Array_t3B8F006A80A48C83471DE3516556F1AFE6873381 
{
	union
	{
		struct
		{
		};
		uint8_t Array_t3B8F006A80A48C83471DE3516556F1AFE6873381__padding[1];
	};
};
struct StreamingStateData_tB4ECCAB2ECCD0555A32604F161D9B80D9FA42B3A 
{
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			uint64_t ___Acc;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint64_t ___Acc_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___Buffer_OffsetPadding[64];
			uint8_t ___Buffer;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___Buffer_OffsetPadding_forAlignmentOnly[64];
			uint8_t ___Buffer_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___IsHash64_OffsetPadding[320];
			int32_t ___IsHash64;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___IsHash64_OffsetPadding_forAlignmentOnly[320];
			int32_t ___IsHash64_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___BufferedSize_OffsetPadding[324];
			int32_t ___BufferedSize;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___BufferedSize_OffsetPadding_forAlignmentOnly[324];
			int32_t ___BufferedSize_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___NbStripesSoFar_OffsetPadding[328];
			int32_t ___NbStripesSoFar;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___NbStripesSoFar_OffsetPadding_forAlignmentOnly[328];
			int32_t ___NbStripesSoFar_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___TotalLength_OffsetPadding[336];
			int64_t ___TotalLength;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___TotalLength_OffsetPadding_forAlignmentOnly[336];
			int64_t ___TotalLength_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___Seed_OffsetPadding[344];
			uint64_t ___Seed;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___Seed_OffsetPadding_forAlignmentOnly[344];
			uint64_t ___Seed_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___SecretKey_OffsetPadding[352];
			uint8_t ___SecretKey;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___SecretKey_OffsetPadding_forAlignmentOnly[352];
			uint8_t ___SecretKey_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ____PadEnd_OffsetPadding[540];
			uint8_t ____PadEnd;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ____PadEnd_OffsetPadding_forAlignmentOnly[540];
			uint8_t ____PadEnd_forAlignmentOnly;
		};
	};
};
struct AlignOfHelper_1_t1C2506D8F2F53BF0A585917F05CD8BC880EBB752 
{
	uint8_t ___dummy;
	intptr_t ___data;
};
struct AlignOfHelper_1_tB6C5A6C6C3F87E6E89F1D273846EEC6B31DFCDA9 
{
	uint8_t ___dummy;
	UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926 ___data;
};
struct AlignOfHelper_1_t71DBCA88B2EAABDA2DD3387E09C204E5E9B12EEA 
{
	uint8_t ___dummy;
	UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C ___data;
};
struct HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4 
{
	uint8_t* ___Ptr;
	EntityId_t982FBD037EAC5CA077B1602A7EA40E3523AA0FC8* ___Keys;
	int32_t* ___Next;
	int32_t* ___Buckets;
	int32_t ___Count;
	int32_t ___Capacity;
	int32_t ___Log2MinGrowth;
	int32_t ___BucketCapacity;
	int32_t ___AllocatedIndex;
	int32_t ___FirstFreeIdx;
	int32_t ___SizeOfTValue;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___Allocator;
};
struct HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B 
{
	uint8_t* ___Ptr;
	int32_t* ___Keys;
	int32_t* ___Next;
	int32_t* ___Buckets;
	int32_t ___Count;
	int32_t ___Capacity;
	int32_t ___Log2MinGrowth;
	int32_t ___BucketCapacity;
	int32_t ___AllocatedIndex;
	int32_t ___FirstFreeIdx;
	int32_t ___SizeOfTValue;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___Allocator;
};
struct UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258 
{
	UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2* ___m_Buffer;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___m_AllocatorLabel;
};
struct UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD 
{
	UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2* ___m_Buffer;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___m_AllocatorLabel;
};
struct UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515 
{
	UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2* ___m_Buffer;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___m_AllocatorLabel;
};
struct UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6 
{
	int32_t* ___Ptr;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___Allocator;
	int32_t ___m_Capacity;
	int32_t ___m_Filled;
	int32_t ___m_Write;
	int32_t ___m_Read;
};
struct Allocator_t996642592271AAD9EE688F142741D512C07B5824 
{
	int32_t ___value__;
};
struct NativeArrayOptions_t3E979EEF4B4840228A7692A97DA07553C6465F1D 
{
	int32_t ___value__;
};
struct UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4 
{
	uint64_t* ___Ptr;
	int32_t ___Length;
	int32_t ___Capacity;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___Allocator;
};
struct UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2 
{
	intptr_t ___m_FirstBlock;
	intptr_t ___m_LastBlock;
	int32_t ___m_MaxItems;
	int32_t ___m_CurrentRead;
	uint8_t* ___m_CurrentWriteBlockTLS;
};
struct UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C 
{
	UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C* ___Next;
	U3CDataU3Ee__FixedBuffer_t60155FEDF2B8430C4B466CB3CDD85327C03F1116 ___Data;
};
struct UntypedUnsafeList_tB7A46F76589C71832F1147292E5123FB99E199B2 
{
	void* ___Ptr;
	int32_t ___m_length;
	int32_t ___m_capacity;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___Allocator;
	int32_t ___padding;
};
struct Range_tB5BAD1274CA0989FC97B0093B4149EF3CD5F21AC 
{
	intptr_t ___Pointer;
	int32_t ___Items;
	AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___Allocator;
};
struct StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020 
{
	StreamingStateData_tB4ECCAB2ECCD0555A32604F161D9B80D9FA42B3A ___State;
};
struct AlignOfHelper_1_t273EAC6E531567E9A44C7A35B5D7DABDD659FC23 
{
	uint8_t ___dummy;
	HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4 ___data;
};
struct AlignOfHelper_1_t245EB4127E14738DE7A1928AA9BD796398AB3E50 
{
	uint8_t ___dummy;
	HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B ___data;
};
struct AlignOfHelper_1_t7423A7512EC617A3595A4C8B8D8B41727DCB2163 
{
	uint8_t ___dummy;
	UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258 ___data;
};
struct AlignOfHelper_1_t7A6E7D9FBDB65F6D78B7607E4870FA0A1580A4D8 
{
	uint8_t ___dummy;
	UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD ___data;
};
struct AlignOfHelper_1_t3E9E85B92672E4CC618A0D4B574EDF6FD77224BE 
{
	uint8_t ___dummy;
	UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515 ___data;
};
struct AlignOfHelper_1_tC0471DA3EB3B5C4A559BBA96E11DC778D51C2693 
{
	uint8_t ___dummy;
	UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6 ___data;
};
struct AlignOfHelper_1_tDBD984B5788DF4AA3E90F8C09B5859ED6CB5D981 
{
	uint8_t ___dummy;
	UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4 ___data;
};
struct AlignOfHelper_1_tE427A888F6FDAAB27509B03069802920EC4AA0A3 
{
	uint8_t ___dummy;
	UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2 ___data;
};
struct NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18 
{
	void* ___m_Buffer;
	int32_t ___m_Length;
	int32_t ___m_AllocatorLabel;
};
struct MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 
{
	union
	{
		struct
		{
			intptr_t ___pointer;
			int32_t ___allocator;
		};
		uint8_t MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2__padding[16];
	};
};
struct UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67 
{
	UntypedUnsafeList_tB7A46F76589C71832F1147292E5123FB99E199B2 ___m_UntypedListData;
};
struct Block_tCCF620817FE305B5BF7B0FB7705B4571F976C4E3 
{
	Range_tB5BAD1274CA0989FC97B0093B4149EF3CD5F21AC ___Range;
	int32_t ___BytesPerItem;
	int32_t ___AllocatedItems;
	uint8_t ___Log2Alignment;
	uint8_t ___Padding0;
	uint16_t ___Padding1;
	uint32_t ___Padding2;
};
struct AlignOfHelper_1_t89E0918ECFFC8A376F89EAE40DA2D8C58E85432E 
{
	uint8_t ___dummy;
	UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67 ___data;
};
struct MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84 
{
	uint8_t* ___m_pointer;
	int64_t ___m_bytes;
	Union_t651696C44E98CE15C53FE2628FA73E4B102D6C21 ___m_union;
	MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 ___m_memoryLabel;
};
struct Reader_tE85B398678D2471A73C94A5DCAECD9AA2CAA4458 
{
	Block_tCCF620817FE305B5BF7B0FB7705B4571F976C4E3 ___m_BlockData;
	UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C* ___m_CurrentBlock;
	uint8_t* ___m_CurrentPtr;
	uint8_t* ___m_CurrentBlockEnd;
	int32_t ___m_RemainingItemCount;
	int32_t ___m_LastBlockSize;
};
struct Writer_t16BC5D7104F5959779BA8C578EC794067872FB0F 
{
	Block_tCCF620817FE305B5BF7B0FB7705B4571F976C4E3 ___m_BlockData;
	UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C* ___m_CurrentBlock;
	uint8_t* ___m_CurrentPtr;
	uint8_t* ___m_CurrentBlockEnd;
	int32_t ___m_ForeachIndex;
	int32_t ___m_ElementCount;
	UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C* ___m_FirstBlock;
	int32_t ___m_FirstOffset;
	int32_t ___m_NumberOfBlocks;
	int32_t ___m_ThreadIndex;
};
struct AlignOfHelper_1_t3AC3A908E2A7B1D4167756986BD005C60ACF4BC9 
{
	uint8_t ___dummy;
	MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84 ___data;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	String_t* ___TrueString;
	String_t* ___FalseString;
};
struct IntPtr_t_StaticFields
{
	intptr_t ___Zero;
};
struct StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020_StaticFields
{
	int32_t ___SECRET_LIMIT;
	int32_t ___NB_STRIPES_PER_BLOCK;
	int32_t ___INTERNAL_BUFFER_SIZE;
	int32_t ___INTERNAL_BUFFER_STRIPES;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif


IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7_mE0C65F4221E48BB07F75856972A0CCDAE284AB9F (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A_mC8EF710494C8986B279262C6C64947BE2C1DC55C (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_m1AB8BF0921C24186B3C43FC4341AAD2F6FE87C4D (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector3Int_t65CB06F557251D18A37BD71F3655BA836A357376_mB00220519A80AA5ACE36B7DBA2E6A6CB796302AF (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3_mED1DC338E5DB62F3E64F6272A9F12150DA5CC1B9 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisIl2CppSharedGenericObject_m2137DD4D5FD5998392651B678C11AE04FC8ABFC3_gshared (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m274F833CD001C63E56A22D8F42C76CE8C6CC39DF_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mFBA66C06ECEB0A2BC1AAE634B6E6BB436F957084_inline (void* ___0_source, int32_t ___1_index, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtility_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mD9A1240B1BCA4029B93CBDCE0196491FEC192E38_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Reader_ReadNext_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m008862EE68B89223188A3A1C95A528DC5CD56774 (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* Array_Resize_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m66D39A0700C852DFD578157D9E4862A8230233F3 (MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m0593078BB9DC842E2FE2C88E802B606CDC972402_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m9D33FAB445EBAAEAB91BED2D158B3290B610D47E_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_m796E3C1D5F3A55A9EFFD1AA98C09AB788198C7A1_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_m70E5F2C375F3B4039114929EBDF86DFCBF1EF99C_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m756E0E44BD2515C8949896575600531B61F5B16B_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m219DC3BB1EF8E9E18A66470555A9974E2C7D9EC6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mB152D3C658FB48DEDB6E662EEB0C91018E7F2857_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisIntPtr_t_mB17C6C168D8CA088879EDD0D5B9923F70F5269B8_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m8127223B00E08567EC09EF1D992F89C74F248AB0_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m56B5B812224539B0C633BABE891ADDE1973580F6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mC1EBD5161294EF95B8AC378DDF616869D287DC36_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m3DF404928C7038CCD5C05BBBA41C4B2FD35A4253_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_mD84C2AFFB4ED036B913CEAD2FBF8A60E9C3A017D_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m5892DF1B875736B0E3612716E560A4E4AD5F6591_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m6AD8B4EF31CDD94CA734378B1443806DAFE68A05_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mB5611745B3ED473868EF6B61479E2860BBB2223C_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m0B7B877C8F00DCC1C95D7124B3B0D83C6D492345_inline (void* ___0_source, int32_t ___1_index, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtilityInternal_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mECC3B0123B8C95AF837F45785072BE5466D989C9_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m32D4BAEAEAB8B63CD58530C3821D9E05084CFD66_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m5DB7C8854924D1E1D01E14FB83112EBF8905533C_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mD30E57AEFF1E2218A4DCE4EA77B9AE170B7A6BB3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mB3311E15857CF490132959FAA501B173670237CA_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m00C87F308E8C18DB5D47FA0A4A1E8D5738894086_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_mB76E5D04768F570DF3F45ABEB65D6BAFDFFF638A_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mC44B74F48735FA62BAE3FBA763123E932B9EF822_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisIntPtr_t_m7FF5CB3788B437CA624174389D25EE9BCB2B212E_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m29B7D9DA9527706BBA1F73ECEA934CCD0B2D99E3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_mA359E62F880352C70AB4DC921D67E178E6DACB15_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mD2E2A270320B9190D3ECE518E4055D43CFF0C7EC_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m73AE74F3CDDD5919B822BA2521D23FF3D3C930E6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m3CE37249D5247521F44E2D498FDBC3714462A3E3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m78688762B313B8CD8725C345D0D2446E999924E4_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m844535A4D039E811CA8C8A80A30A2499268FF979_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t676424B8757032427D7761BC732079DC7512D8AE_mB243DC326B2D814D1A62FDABCC2F2AA541A8C895_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mA24CDBA02971DC06DEDE40B17E72F8C262B6B131_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtilityInternal_InternalCopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m1ADEAC45E92C10A0C3D9686063C582FDD76F5937_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t273EAC6E531567E9A44C7A35B5D7DABDD659FC23_m2FCB00D512044D1C5DB62954E2684D67CB44E6C3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m4AEA1F2D0EBC5D0A1C2819C02415E77F093D96E2_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t245EB4127E14738DE7A1928AA9BD796398AB3E50_m7E4E66C593913FCDFC8C03C06EFC4FFDC9A96BCF_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m0E37BE54FFDD9D93D5ADCA4464466720A0534F51_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7423A7512EC617A3595A4C8B8D8B41727DCB2163_m4D97EFDBFE98987E636ECEAD022BD99CAB158A50_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mB7E3DB3D712EE116DD131EEFE9548C1E0FA09AF9_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7A6E7D9FBDB65F6D78B7607E4870FA0A1580A4D8_m7D90BECD86875EEF05E1DAAF5AD5FDBA356223E1_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mD5CCCB7294CE98231752303CB481A5D7B11E89E6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3E9E85B92672E4CC618A0D4B574EDF6FD77224BE_m74FE4A857995F87CEB1B9C21DC301B1AC06DF227_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_mA7825F57E8BE201BAAE0B00BAA31C1A79D6140D6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tC0471DA3EB3B5C4A559BBA96E11DC778D51C2693_m28A0205203C49D5369496BB8258836A6E90B4275_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m6C73B80F6290ED267404794EB2AC27F5EEDB4EF3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tAF795AF837E3C37401B92C7483D74A4E5145DB4D_m8CB43E8118CC0E65E6E8130381DEB11701F21848_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_m485E79FA6794AD30B29C6414A85A867DF156E5E1_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t1C2506D8F2F53BF0A585917F05CD8BC880EBB752_m6197C4317CE4E01732509D53E08172FEA65ED04E_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisIntPtr_t_mF9218FE174B91F132731D032A744AFC048AFB932_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE21040F72E90527C319FF9B07B398E23DF95BE2E_m2D3BC72BBF45866A72C7ADF31F40AEFC56BBD498_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_mAA2B8333B77A7410376516B6E13FCC02AEE6A5AC_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tDBD984B5788DF4AA3E90F8C09B5859ED6CB5D981_m0E4242520596E02B9AAE81652FFB8EE3542259AA_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m9B75EBBD9DF8560D30B3C2E1E2C38BF7D85CAF9D_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tB6C5A6C6C3F87E6E89F1D273846EEC6B31DFCDA9_m98AA8DA7813F89F461C97A6373493AB6D70B858B_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_m3C94249F648A50E2D27EDAD82BD9BFD652086097_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t71DBCA88B2EAABDA2DD3387E09C204E5E9B12EEA_m05924C78C0CB3BE4A7F3029ACEA5F044E6E559A0_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m4443406CB5E8C38BA71CE11444CF35BD26797053_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE427A888F6FDAAB27509B03069802920EC4AA0A3_mE4962E35BBA3FE3704E13AB652950BF09D05115F_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m9D71081BBF4C1B8CCA23832463B32C5362F17249_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t89E0918ECFFC8A376F89EAE40DA2D8C58E85432E_mFD07D65557C1FB04588D27C6A200F7746663E0B0_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_mA5DC0B2032262984D97EFF4CF894E776A34A181F_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3AC3A908E2A7B1D4167756986BD005C60ACF4BC9_m0307FE22A5AF62C62045020268823FBD9652FA71_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m64096CFB58B4B0A976A42BC41901B89806B85F69_inline (const RuntimeMethod* method) ;

IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7_mE0C65F4221E48BB07F75856972A0CCDAE284AB9F (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A_mC8EF710494C8986B279262C6C64947BE2C1DC55C (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_m1AB8BF0921C24186B3C43FC4341AAD2F6FE87C4D (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector3Int_t65CB06F557251D18A37BD71F3655BA836A357376_mB00220519A80AA5ACE36B7DBA2E6A6CB796302AF (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TypeUtility_CreateTypeConstructor_TisVector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3_mED1DC338E5DB62F3E64F6272A9F12150DA5CC1B9 (const RuntimeMethod* method) ;
inline RuntimeObject* TypeUtility_CreateTypeConstructor_TisIl2CppSharedGenericObject_m2137DD4D5FD5998392651B678C11AE04FC8ABFC3 (const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (const RuntimeMethod*))TypeUtility_CreateTypeConstructor_TisIl2CppSharedGenericObject_m2137DD4D5FD5998392651B678C11AE04FC8ABFC3_gshared)(method);
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m274F833CD001C63E56A22D8F42C76CE8C6CC39DF_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CollectionHelper_IsAligned_m4761669B9084B892256349C0FF27DBF494DA9AE9 (uint64_t ___0_offset, int32_t ___1_alignmentPowerOfTwo, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mFBA66C06ECEB0A2BC1AAE634B6E6BB436F957084_inline (void* ___0_source, int32_t ___1_index, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnsafeUtility_MemCpy_m5CEA91ACDADC522E584AE3A2AB2B0B74393A9177 (void* ___0_destination, void* ___1_source, int64_t ___2_size, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtility_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mD9A1240B1BCA4029B93CBDCE0196491FEC192E38_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Reader_ReadNext_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m008862EE68B89223188A3A1C95A528DC5CD56774 (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* Reader_ReadNext_m7C9150A60E622C8581A3E80C8DD1AD71D043BF6A (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, int32_t ___0_structSize, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* Reader_ReadUnsafePtr_m6CDEF727114990BC4C2DF321BE752F229104A888 (Reader_tE85B398678D2471A73C94A5DCAECD9AA2CAA4458* __this, int32_t ___0_size, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* Writer_Allocate_m85F9D4FF8746C0A59DC23A9003398A6D9D42526D (Writer_t16BC5D7104F5959779BA8C578EC794067872FB0F* __this, int32_t ___0_size, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StreamingState_Update_mD0C172FBC9AB99BC72DF8CC3665ABB61658AB9E5 (StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020* __this, void* ___0_input, int32_t ___1_length, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* Array_Resize_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m66D39A0700C852DFD578157D9E4862A8230233F3 (MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m0593078BB9DC842E2FE2C88E802B606CDC972402_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380 (void* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, int64_t ___4_size, int32_t ___5_align, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m9D33FAB445EBAAEAB91BED2D158B3290B610D47E_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_m796E3C1D5F3A55A9EFFD1AA98C09AB788198C7A1_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_m70E5F2C375F3B4039114929EBDF86DFCBF1EF99C_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m756E0E44BD2515C8949896575600531B61F5B16B_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m219DC3BB1EF8E9E18A66470555A9974E2C7D9EC6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mB152D3C658FB48DEDB6E662EEB0C91018E7F2857_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* Array_Resize_mA60C117B1C01C122EB2D509DD4EADE4FB44C2150 (void* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 ___3_label, int64_t ___4_size, int32_t ___5_align, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisIntPtr_t_mB17C6C168D8CA088879EDD0D5B9923F70F5269B8_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m8127223B00E08567EC09EF1D992F89C74F248AB0_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m56B5B812224539B0C633BABE891ADDE1973580F6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mC1EBD5161294EF95B8AC378DDF616869D287DC36_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m3DF404928C7038CCD5C05BBBA41C4B2FD35A4253_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_mD84C2AFFB4ED036B913CEAD2FBF8A60E9C3A017D_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m5892DF1B875736B0E3612716E560A4E4AD5F6591_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m6AD8B4EF31CDD94CA734378B1443806DAFE68A05_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mB5611745B3ED473868EF6B61479E2860BBB2223C_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m0B7B877C8F00DCC1C95D7124B3B0D83C6D492345_inline (void* ___0_source, int32_t ___1_index, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtilityInternal_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mECC3B0123B8C95AF837F45785072BE5466D989C9_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m32D4BAEAEAB8B63CD58530C3821D9E05084CFD66_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m5DB7C8854924D1E1D01E14FB83112EBF8905533C_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mD30E57AEFF1E2218A4DCE4EA77B9AE170B7A6BB3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mB3311E15857CF490132959FAA501B173670237CA_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m00C87F308E8C18DB5D47FA0A4A1E8D5738894086_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_mB76E5D04768F570DF3F45ABEB65D6BAFDFFF638A_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mC44B74F48735FA62BAE3FBA763123E932B9EF822_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisIntPtr_t_m7FF5CB3788B437CA624174389D25EE9BCB2B212E_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m29B7D9DA9527706BBA1F73ECEA934CCD0B2D99E3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_mA359E62F880352C70AB4DC921D67E178E6DACB15_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mD2E2A270320B9190D3ECE518E4055D43CFF0C7EC_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m73AE74F3CDDD5919B822BA2521D23FF3D3C930E6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m3CE37249D5247521F44E2D498FDBC3714462A3E3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m78688762B313B8CD8725C345D0D2446E999924E4_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m844535A4D039E811CA8C8A80A30A2499268FF979_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t676424B8757032427D7761BC732079DC7512D8AE_mB243DC326B2D814D1A62FDABCC2F2AA541A8C895_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mA24CDBA02971DC06DEDE40B17E72F8C262B6B131_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtilityInternal_InternalCopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m1ADEAC45E92C10A0C3D9686063C582FDD76F5937_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t273EAC6E531567E9A44C7A35B5D7DABDD659FC23_m2FCB00D512044D1C5DB62954E2684D67CB44E6C3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m4AEA1F2D0EBC5D0A1C2819C02415E77F093D96E2_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t245EB4127E14738DE7A1928AA9BD796398AB3E50_m7E4E66C593913FCDFC8C03C06EFC4FFDC9A96BCF_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m0E37BE54FFDD9D93D5ADCA4464466720A0534F51_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7423A7512EC617A3595A4C8B8D8B41727DCB2163_m4D97EFDBFE98987E636ECEAD022BD99CAB158A50_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mB7E3DB3D712EE116DD131EEFE9548C1E0FA09AF9_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7A6E7D9FBDB65F6D78B7607E4870FA0A1580A4D8_m7D90BECD86875EEF05E1DAAF5AD5FDBA356223E1_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mD5CCCB7294CE98231752303CB481A5D7B11E89E6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3E9E85B92672E4CC618A0D4B574EDF6FD77224BE_m74FE4A857995F87CEB1B9C21DC301B1AC06DF227_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_mA7825F57E8BE201BAAE0B00BAA31C1A79D6140D6_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tC0471DA3EB3B5C4A559BBA96E11DC778D51C2693_m28A0205203C49D5369496BB8258836A6E90B4275_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m6C73B80F6290ED267404794EB2AC27F5EEDB4EF3_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tAF795AF837E3C37401B92C7483D74A4E5145DB4D_m8CB43E8118CC0E65E6E8130381DEB11701F21848_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_m485E79FA6794AD30B29C6414A85A867DF156E5E1_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t1C2506D8F2F53BF0A585917F05CD8BC880EBB752_m6197C4317CE4E01732509D53E08172FEA65ED04E_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisIntPtr_t_mF9218FE174B91F132731D032A744AFC048AFB932_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE21040F72E90527C319FF9B07B398E23DF95BE2E_m2D3BC72BBF45866A72C7ADF31F40AEFC56BBD498_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_mAA2B8333B77A7410376516B6E13FCC02AEE6A5AC_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tDBD984B5788DF4AA3E90F8C09B5859ED6CB5D981_m0E4242520596E02B9AAE81652FFB8EE3542259AA_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m9B75EBBD9DF8560D30B3C2E1E2C38BF7D85CAF9D_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tB6C5A6C6C3F87E6E89F1D273846EEC6B31DFCDA9_m98AA8DA7813F89F461C97A6373493AB6D70B858B_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_m3C94249F648A50E2D27EDAD82BD9BFD652086097_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t71DBCA88B2EAABDA2DD3387E09C204E5E9B12EEA_m05924C78C0CB3BE4A7F3029ACEA5F044E6E559A0_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m4443406CB5E8C38BA71CE11444CF35BD26797053_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE427A888F6FDAAB27509B03069802920EC4AA0A3_mE4962E35BBA3FE3704E13AB652950BF09D05115F_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m9D71081BBF4C1B8CCA23832463B32C5362F17249_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t89E0918ECFFC8A376F89EAE40DA2D8C58E85432E_mFD07D65557C1FB04588D27C6A200F7746663E0B0_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_mA5DC0B2032262984D97EFF4CF894E776A34A181F_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3AC3A908E2A7B1D4167756986BD005C60ACF4BC9_m0307FE22A5AF62C62045020268823FBD9652FA71_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m64096CFB58B4B0A976A42BC41901B89806B85F69_inline (const RuntimeMethod* method) ;
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisVector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7_m032CA5A330B4032D164F88B2B40B6A7F2590357F (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = TypeUtility_CreateTypeConstructor_TisVector2_t1FD6F485C871E832B347AB2DC8CBA08B739D8DF7_mE0C65F4221E48BB07F75856972A0CCDAE284AB9F(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)L_0);
		return;
	}
}
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisVector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A_m0E8E39845CA3F1BFADAC1B15E849E5885D813E6F (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = TypeUtility_CreateTypeConstructor_TisVector2Int_t69B2886EBAB732D9B880565E18E7568F3DE0CE6A_mC8EF710494C8986B279262C6C64947BE2C1DC55C(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)L_0);
		return;
	}
}
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisVector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_mA88A16DC097E207AD6CE15E89D3B94DBB01BDACE (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = TypeUtility_CreateTypeConstructor_TisVector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_m1AB8BF0921C24186B3C43FC4341AAD2F6FE87C4D(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)L_0);
		return;
	}
}
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisVector3Int_t65CB06F557251D18A37BD71F3655BA836A357376_m28D19B06528A4614AF9C3E29DC8274DE067868D8 (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = TypeUtility_CreateTypeConstructor_TisVector3Int_t65CB06F557251D18A37BD71F3655BA836A357376_mB00220519A80AA5ACE36B7DBA2E6A6CB796302AF(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)L_0);
		return;
	}
}
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisVector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3_m3CD9CD3C768BBCC83DD02B2C76300A03AF1B1AD6 (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = TypeUtility_CreateTypeConstructor_TisVector4_t58B63D32F48C0DBF50DE2C60794C4676C80EDBE3_mED1DC338E5DB62F3E64F6272A9F12150DA5CC1B9(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)L_0);
		return;
	}
}
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisIl2CppSharedGenericObject_m605856D300205D523BA8BB84C019885CD338F878_gshared (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = TypeUtility_CreateTypeConstructor_TisIl2CppSharedGenericObject_m2137DD4D5FD5998392651B678C11AE04FC8ABFC3(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = (RuntimeObject*)L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)(RuntimeObject*)L_0);
		return;
	}
}
// Method Definition Index: 51960
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TypeConstructorVisitor_Visit_TisIl2CppFullySharedGenericAny_m8B414261A078E3FE7517FAF2F9E944969F6B73E2_fshared (TypeConstructorVisitor_tFC61250F68BF43FE889F5B1002CBFF90648E9C7F* __this, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	CHECKED_LOCAL(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit);
	//<source_info:<no-source>:1>
	{
		CHECKED_LOCAL_INIT(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_StaticInit,(TypeUtility_t62F146E51B66BB18FA0E1EFA84A564FF2B8AAD03_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		RuntimeObject* L_0;
		L_0 = ((  RuntimeObject* (*) (const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))(il2cpp_rgctx_method(method->rgctx_data, 0));
		__this->___TypeConstructor = (RuntimeObject*)L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___TypeConstructor), (void*)(RuntimeObject*)L_0);
		return;
	}
}
// Method Definition Index: 43130
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Reader_ReadNext_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m008862EE68B89223188A3A1C95A528DC5CD56774 (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	void* V_2 = NULL;
	{
		uint32_t L_0 = sizeof(int32_t);
		V_0 = (int32_t)L_0;
		uint8_t* L_1 = __this->___Ptr;
		int32_t L_2 = __this->___Offset;
		V_2 = (void*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_1, L_2));
		void* L_3 = V_2;
		uint64_t L_4 = (il2cpp_codegen_conv<uint64_t,void*,intptr_t,false,false>(L_3,NULL));
		int32_t L_5;
		L_5 = UnsafeUtility_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m274F833CD001C63E56A22D8F42C76CE8C6CC39DF_inline(NULL);
		bool L_6;
		L_6 = CollectionHelper_IsAligned_m4761669B9084B892256349C0FF27DBF494DA9AE9(L_4, L_5, NULL);
		if (!L_6)
		{
			goto IL_002d;
		}
	}
	{
		void* L_7 = V_2;
		int32_t L_8;
		L_8 = UnsafeUtility_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mFBA66C06ECEB0A2BC1AAE634B6E6BB436F957084_inline(L_7, 0, NULL);
		V_1 = L_8;
		goto IL_0038;
	}

IL_002d:
	{
		uintptr_t L_9 = (il2cpp_codegen_conv<uintptr_t,int32_t*,intptr_t,false,false>((&V_1),NULL));
		void* L_10 = V_2;
		int32_t L_11 = V_0;
		int64_t L_12 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_11,NULL));
		UnsafeUtility_MemCpy_m5CEA91ACDADC522E584AE3A2AB2B0B74393A9177((void*)L_9, L_10, L_12, NULL);
	}

IL_0038:
	{
		int32_t L_13 = __this->___Offset;
		int32_t L_14 = V_0;
		__this->___Offset = ((int32_t)il2cpp_codegen_add(L_13, L_14));
		int32_t L_15 = V_1;
		return L_15;
	}
}
// Method Definition Index: 43130
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Reader_ReadNext_TisIl2CppFullySharedGenericStruct_m4BEED82D6907F7A1B30916AD15A551B4477AB2C2_fshared (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, Il2CppFullySharedGenericStruct* il2cppRetVal, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	const Il2CppFullySharedGenericStruct L_8 = alloca(SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49);
	const Il2CppFullySharedGenericStruct L_15 = L_8;
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	Il2CppFullySharedGenericStruct V_1 = alloca(SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49);
	memset(V_1, 0, SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49);
	void* V_2 = NULL;
	{
		uint32_t L_0 = SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49;
		V_0 = (int32_t)L_0;
		uint8_t* L_1 = __this->___Ptr;
		int32_t L_2 = __this->___Offset;
		V_2 = (void*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_1, L_2));
		void* L_3 = V_2;
		uint64_t L_4 = (il2cpp_codegen_conv<uint64_t,void*,intptr_t,false,false>(L_3,NULL));
		int32_t L_5;
		L_5 = ((  int32_t (*) (const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))(il2cpp_rgctx_method(method->rgctx_data, 1));
		bool L_6;
		L_6 = CollectionHelper_IsAligned_m4761669B9084B892256349C0FF27DBF494DA9AE9(L_4, L_5, NULL);
		if (!L_6)
		{
			goto IL_002d;
		}
	}
	{
		void* L_7 = V_2;
		InvokerActionInvoker3< void*, int32_t, Il2CppFullySharedGenericStruct* >::Invoke(il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 2)), il2cpp_rgctx_method(method->rgctx_data, 2), NULL, L_7, 0, (Il2CppFullySharedGenericStruct*)L_8);
		il2cpp_codegen_memcpy(V_1, L_8, SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49);
		goto IL_0038;
	}

IL_002d:
	{
		uintptr_t L_9 = (il2cpp_codegen_conv<uintptr_t,Il2CppFullySharedGenericStruct*,intptr_t,false,false>((Il2CppFullySharedGenericStruct*)V_1,NULL));
		void* L_10 = V_2;
		int32_t L_11 = V_0;
		int64_t L_12 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_11,NULL));
		UnsafeUtility_MemCpy_m5CEA91ACDADC522E584AE3A2AB2B0B74393A9177((void*)L_9, L_10, L_12, NULL);
	}

IL_0038:
	{
		int32_t L_13 = __this->___Offset;
		int32_t L_14 = V_0;
		__this->___Offset = ((int32_t)il2cpp_codegen_add(L_13, L_14));
		il2cpp_codegen_memcpy(L_15, V_1, SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49);
		il2cpp_codegen_memcpy(il2cppRetVal, L_15, SizeOf_T_t23EFE765355235E0552FEAA74620709E7D407C49);
		return;
	}
}
// Method Definition Index: 43129
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Reader_ReadNext_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m17201F199E61247BB0595C13E633DDC1D94569F9 (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, int32_t* ___0_value, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	void* V_1 = NULL;
	void* V_2 = NULL;
	int32_t* V_3 = NULL;
	{
		uint32_t L_0 = sizeof(int32_t);
		V_0 = (int32_t)L_0;
		uint8_t* L_1 = __this->___Ptr;
		int32_t L_2 = __this->___Offset;
		V_1 = (void*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_1, L_2));
		void* L_3 = V_1;
		uint64_t L_4 = (il2cpp_codegen_conv<uint64_t,void*,intptr_t,false,false>(L_3,NULL));
		int32_t L_5;
		L_5 = UnsafeUtility_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m274F833CD001C63E56A22D8F42C76CE8C6CC39DF_inline(NULL);
		bool L_6;
		L_6 = CollectionHelper_IsAligned_m4761669B9084B892256349C0FF27DBF494DA9AE9(L_4, L_5, NULL);
		if (!L_6)
		{
			goto IL_002c;
		}
	}
	{
		void* L_7 = V_1;
		int32_t* L_8 = ___0_value;
		UnsafeUtility_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mD9A1240B1BCA4029B93CBDCE0196491FEC192E38_inline(L_7, L_8, NULL);
		goto IL_003d;
	}

IL_002c:
	{
		int32_t* L_9 = ___0_value;
		V_3 = L_9;
		int32_t* L_10 = V_3;
		uintptr_t L_11 = (il2cpp_codegen_conv<uintptr_t,int32_t*,intptr_t,false,false>(L_10,NULL));
		V_2 = (void*)L_11;
		void* L_12 = V_2;
		void* L_13 = V_1;
		int32_t L_14 = V_0;
		int64_t L_15 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_14,NULL));
		UnsafeUtility_MemCpy_m5CEA91ACDADC522E584AE3A2AB2B0B74393A9177(L_12, L_13, L_15, NULL);
		uintptr_t L_16 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		V_3 = (int32_t*)L_16;
	}

IL_003d:
	{
		int32_t L_17 = __this->___Offset;
		int32_t L_18 = V_0;
		__this->___Offset = ((int32_t)il2cpp_codegen_add(L_17, L_18));
		return;
	}
}
// Method Definition Index: 43129
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Reader_ReadNext_TisIl2CppFullySharedGenericStruct_mD61521649968D72362C4D143C6798CF697284C59_fshared (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, Il2CppFullySharedGenericStruct* ___0_value, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t989833F3B8437253060D8003F814134F2C820AF6 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	void* V_1 = NULL;
	void* V_2 = NULL;
	Il2CppFullySharedGenericStruct* V_3 = NULL;
	{
		uint32_t L_0 = SizeOf_T_t989833F3B8437253060D8003F814134F2C820AF6;
		V_0 = (int32_t)L_0;
		uint8_t* L_1 = __this->___Ptr;
		int32_t L_2 = __this->___Offset;
		V_1 = (void*)((uint8_t*)il2cpp_codegen_add((intptr_t)L_1, L_2));
		void* L_3 = V_1;
		uint64_t L_4 = (il2cpp_codegen_conv<uint64_t,void*,intptr_t,false,false>(L_3,NULL));
		int32_t L_5;
		L_5 = ((  int32_t (*) (const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))(il2cpp_rgctx_method(method->rgctx_data, 1));
		bool L_6;
		L_6 = CollectionHelper_IsAligned_m4761669B9084B892256349C0FF27DBF494DA9AE9(L_4, L_5, NULL);
		if (!L_6)
		{
			goto IL_002c;
		}
	}
	{
		void* L_7 = V_1;
		Il2CppFullySharedGenericStruct* L_8 = ___0_value;
		((  void (*) (void*, Il2CppFullySharedGenericStruct*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 2)))(L_7, L_8, il2cpp_rgctx_method(method->rgctx_data, 2));
		goto IL_003d;
	}

IL_002c:
	{
		Il2CppFullySharedGenericStruct* L_9 = ___0_value;
		V_3 = L_9;
		Il2CppFullySharedGenericStruct* L_10 = V_3;
		uintptr_t L_11 = (il2cpp_codegen_conv<uintptr_t,Il2CppFullySharedGenericStruct*,intptr_t,false,false>(L_10,NULL));
		V_2 = (void*)L_11;
		void* L_12 = V_2;
		void* L_13 = V_1;
		int32_t L_14 = V_0;
		int64_t L_15 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_14,NULL));
		UnsafeUtility_MemCpy_m5CEA91ACDADC522E584AE3A2AB2B0B74393A9177(L_12, L_13, L_15, NULL);
		uintptr_t L_16 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		V_3 = (Il2CppFullySharedGenericStruct*)L_16;
	}

IL_003d:
	{
		int32_t L_17 = __this->___Offset;
		int32_t L_18 = V_0;
		__this->___Offset = ((int32_t)il2cpp_codegen_add(L_17, L_18));
		return;
	}
}
// Method Definition Index: 43132
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Reader_ReadNext_TisIl2CppFullySharedGenericStruct_m08EA2C685C92045455508A2CEA28AF34C49BF372_fshared (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18* ___0_value, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___1_allocator, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t3582FAB8EED36663C15B18083F3C38CC8CB2089F = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 1));
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	void* V_2 = NULL;
	{
		int32_t L_0;
		L_0 = Reader_ReadNext_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m008862EE68B89223188A3A1C95A528DC5CD56774(__this, NULL);
		V_0 = L_0;
		NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18* L_1 = ___0_value;
		int32_t L_2 = V_0;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___1_allocator;
		NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18 L_4;
		L_4 = ((  NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18 (*) (int32_t, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148, int32_t, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))(L_2, L_3, 0, il2cpp_rgctx_method(method->rgctx_data, 0));
		*(NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18*)L_1 = L_4;
		int32_t L_5 = V_0;
		uint32_t L_6 = SizeOf_T_t3582FAB8EED36663C15B18083F3C38CC8CB2089F;
		V_1 = ((int32_t)il2cpp_codegen_multiply(L_5, (int32_t)L_6));
		int32_t L_7 = V_1;
		if ((((int32_t)L_7) <= ((int32_t)0)))
		{
			goto IL_003d;
		}
	}
	{
		int32_t L_8 = V_1;
		void* L_9;
		L_9 = Reader_ReadNext_m7C9150A60E622C8581A3E80C8DD1AD71D043BF6A(__this, L_8, NULL);
		V_2 = L_9;
		NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18* L_10 = ___0_value;
		NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18 L_11 = (*(NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18*)L_10);
		void* L_12;
		L_12 = ((  void* (*) (NativeArray_1_tDB8B8DC66CC8E16ED6D9A8C75D2C1AFC80AC1E18, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 2)))(L_11, NULL);
		void* L_13 = V_2;
		int32_t L_14 = V_1;
		int64_t L_15 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_14,NULL));
		UnsafeUtility_MemCpy_m5CEA91ACDADC522E584AE3A2AB2B0B74393A9177(L_12, L_13, L_15, NULL);
	}

IL_003d:
	{
		return;
	}
}
// Method Definition Index: 43133
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* Reader_ReadNextArray_TisIl2CppFullySharedGenericStruct_mE1C3BA7C6F410106344DD729D6EBF3FB62CB67CC_fshared (Reader_t409B401DB3A9E3F5C637DF66E5FAB1F63C291CB6* __this, int32_t* ___0_length, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t6E0E95C00360B98AD1C3846ED50206DFDE012AFF = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	{
		int32_t* L_0 = ___0_length;
		int32_t L_1;
		L_1 = Reader_ReadNext_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m008862EE68B89223188A3A1C95A528DC5CD56774(__this, NULL);
		il2cpp_codegen_stind<int32_t>((int32_t*)L_0, (int32_t)L_1);
		int32_t* L_2 = ___0_length;
		int32_t L_3 = il2cpp_codegen_ldind<int32_t, int32_t>(L_2);
		if (!L_3)
		{
			goto IL_001c;
		}
	}
	{
		int32_t* L_4 = ___0_length;
		int32_t L_5 = il2cpp_codegen_ldind<int32_t, int32_t>(L_4);
		uint32_t L_6 = SizeOf_T_t6E0E95C00360B98AD1C3846ED50206DFDE012AFF;
		void* L_7;
		L_7 = Reader_ReadNext_m7C9150A60E622C8581A3E80C8DD1AD71D043BF6A(__this, ((int32_t)il2cpp_codegen_multiply(L_5, (int32_t)L_6)), NULL);
		return L_7;
	}

IL_001c:
	{
		uintptr_t L_8 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		return (void*)(L_8);
	}
}
// Method Definition Index: 43848
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Reader_Peek_TisIl2CppFullySharedGenericStruct_m9282B5560EB03FCC4C6F0F8AC634AE5A13B555D6_fshared (Reader_tE85B398678D2471A73C94A5DCAECD9AA2CAA4458* __this, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t78876001400B6CD8FBC8DB6052800D0B07D5996E = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	uint8_t* V_1 = NULL;
	{
		uint32_t L_0 = SizeOf_T_t78876001400B6CD8FBC8DB6052800D0B07D5996E;
		V_0 = (int32_t)L_0;
		uint8_t* L_1 = __this->___m_CurrentPtr;
		V_1 = L_1;
		uint8_t* L_2 = V_1;
		int32_t L_3 = V_0;
		uint8_t* L_4 = __this->___m_CurrentBlockEnd;
		if ((!(((uintptr_t)((uint8_t*)il2cpp_codegen_add((intptr_t)L_2, L_3))) > ((uintptr_t)L_4))))
		{
			goto IL_0030;
		}
	}
	{
		UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C* L_5 = __this->___m_CurrentBlock;
		NullCheck(L_5);
		UnsafeStreamBlock_t0DE96C782B2CBF67BE34978C364781E170B3A86C* L_6 = L_5->___Next;
		NullCheck(L_6);
		U3CDataU3Ee__FixedBuffer_t60155FEDF2B8430C4B466CB3CDD85327C03F1116* L_7 = (U3CDataU3Ee__FixedBuffer_t60155FEDF2B8430C4B466CB3CDD85327C03F1116*)(&L_6->___Data);
		uint8_t* L_8 = (uint8_t*)(&L_7->___FixedElementField);
		uintptr_t L_9 = (il2cpp_codegen_conv<uintptr_t,uint8_t*,intptr_t,false,false>(L_8,NULL));
		V_1 = (uint8_t*)L_9;
	}

IL_0030:
	{
		uint8_t* L_10 = V_1;
		Il2CppFullySharedGenericStruct* L_11;
		L_11 = ((  Il2CppFullySharedGenericStruct* (*) (void*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))((void*)L_10, il2cpp_rgctx_method(method->rgctx_data, 1));
		return L_11;
	}
}
// Method Definition Index: 43847
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Reader_Read_TisIl2CppFullySharedGenericStruct_mBBF673BF9BED0383144F187B6541B532F2B6A633_fshared (Reader_tE85B398678D2471A73C94A5DCAECD9AA2CAA4458* __this, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_tB637578F0BD33C09FC40FC64F02C0A2D558139E3 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	{
		uint32_t L_0 = SizeOf_T_tB637578F0BD33C09FC40FC64F02C0A2D558139E3;
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		uint8_t* L_2;
		L_2 = Reader_ReadUnsafePtr_m6CDEF727114990BC4C2DF321BE752F229104A888(__this, L_1, NULL);
		Il2CppFullySharedGenericStruct* L_3;
		L_3 = ((  Il2CppFullySharedGenericStruct* (*) (void*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))((void*)L_2, il2cpp_rgctx_method(method->rgctx_data, 1));
		return L_3;
	}
}
// Method Definition Index: 43837
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Writer_Allocate_TisIl2CppFullySharedGenericStruct_m4503233E7BE788F93FD1C91B7BE65AC121483DB7_fshared (Writer_t16BC5D7104F5959779BA8C578EC794067872FB0F* __this, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t116435D0785AA26BF8114803668B5077681E8C02 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	int32_t V_0 = 0;
	{
		uint32_t L_0 = SizeOf_T_t116435D0785AA26BF8114803668B5077681E8C02;
		V_0 = (int32_t)L_0;
		int32_t L_1 = V_0;
		uint8_t* L_2;
		L_2 = Writer_Allocate_m85F9D4FF8746C0A59DC23A9003398A6D9D42526D(__this, L_1, NULL);
		Il2CppFullySharedGenericStruct* L_3;
		L_3 = ((  Il2CppFullySharedGenericStruct* (*) (void*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))((void*)L_2, il2cpp_rgctx_method(method->rgctx_data, 1));
		return L_3;
	}
}
// Method Definition Index: 43836
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Writer_Write_TisIl2CppFullySharedGenericStruct_m3255C6DE3AD541593524A784715AE50082831917_fshared (Writer_t16BC5D7104F5959779BA8C578EC794067872FB0F* __this, Il2CppFullySharedGenericStruct ___0_value, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t4D7283DBD6CD71348652C5C25061A381CAB760A7 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 1));
	const Il2CppFullySharedGenericStruct L_1 = alloca(SizeOf_T_t4D7283DBD6CD71348652C5C25061A381CAB760A7);
	//<source_info:<no-source>:1>
	{
		Il2CppFullySharedGenericStruct* L_0;
		L_0 = ((  Il2CppFullySharedGenericStruct* (*) (Writer_t16BC5D7104F5959779BA8C578EC794067872FB0F*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))(__this, il2cpp_rgctx_method(method->rgctx_data, 0));
		il2cpp_codegen_memcpy(L_1, ___0_value, SizeOf_T_t4D7283DBD6CD71348652C5C25061A381CAB760A7);
		il2cpp_codegen_memcpy((Il2CppFullySharedGenericStruct*)L_0, L_1, SizeOf_T_t4D7283DBD6CD71348652C5C25061A381CAB760A7);
		Il2CppCodeGenWriteBarrierForClass(il2cpp_rgctx_data(method->rgctx_data, 1), (void**)(Il2CppFullySharedGenericStruct*)L_0, (void*)L_1);
		return;
	}
}
// Method Definition Index: 43062
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StreamingState_Update_TisIl2CppFullySharedGenericStruct_m301FBB6A80BBA46BB5AC2B0DF206CAB72593DE1D_fshared (StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020* __this, Il2CppFullySharedGenericStruct* ___0_input, const RuntimeMethod* method) 
{
	if (!il2cpp_rgctx_is_initialized(method))
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020_il2cpp_TypeInfo_var);
		il2cpp_rgctx_method_init(method);
	}
	const uint32_t SizeOf_T_t0FAE9973A0C60B49E231E1F7F01F959BFBB59552 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 1));
	CHECKED_LOCAL(StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020_StaticInit);
	//<source_info:<no-source>:1>
	{
		Il2CppFullySharedGenericStruct* L_0 = ___0_input;
		void* L_1;
		L_1 = ((  void* (*) (Il2CppFullySharedGenericStruct*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))(L_0, il2cpp_rgctx_method(method->rgctx_data, 0));
		uint32_t L_2 = SizeOf_T_t0FAE9973A0C60B49E231E1F7F01F959BFBB59552;
		CHECKED_LOCAL_INIT(StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020_StaticInit,(StreamingState_t407BC6BBA6752E76D2075153BF945841B2D06020_il2cpp_TypeInfo_var),il2cpp_codegen_runtime_class_init_inline);
		StreamingState_Update_mD0C172FBC9AB99BC72DF8CC3665ABB61658AB9E5(__this, L_1, (int32_t)L_2, NULL);
		return;
	}
}
// Method Definition Index: 42049
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Array_Allocate_TisIl2CppFullySharedGenericStruct_m040B30CAEA05884CC6EF77610B3FABB3B53AB4AD_fshared (int64_t ___0_count, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 ___1_label, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	//<source_info:<no-source>:1>
	{
		uintptr_t L_0 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_1 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_2 = ___0_count;
		MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 L_3 = ___1_label;
		Il2CppFullySharedGenericStruct* L_4;
		L_4 = ((  Il2CppFullySharedGenericStruct* (*) (Il2CppFullySharedGenericStruct*, int64_t, int64_t, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))((Il2CppFullySharedGenericStruct*)L_0, L_1, L_2, L_3, il2cpp_rgctx_method(method->rgctx_data, 0));
		return L_4;
	}
}
// Method Definition Index: 42048
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Array_Allocate_TisIl2CppFullySharedGenericStruct_m472C3AE183E3C768741B28481DD1F012013B13A6_fshared (int64_t ___0_count, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___1_allocator, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	//<source_info:<no-source>:1>
	{
		uintptr_t L_0 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_1 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_2 = ___0_count;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___1_allocator;
		Il2CppFullySharedGenericStruct* L_4;
		L_4 = ((  Il2CppFullySharedGenericStruct* (*) (Il2CppFullySharedGenericStruct*, int64_t, int64_t, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))((Il2CppFullySharedGenericStruct*)L_0, L_1, L_2, L_3, il2cpp_rgctx_method(method->rgctx_data, 0));
		return L_4;
	}
}
// Method Definition Index: 42048
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* Array_Allocate_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_mCDBD1381CA662F016D4B6003D9BA89FE592EB6D8 (int64_t ___0_count, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___1_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uintptr_t L_0 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_1 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		int64_t L_2 = ___0_count;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___1_allocator;
		MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* L_4;
		L_4 = Array_Resize_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m66D39A0700C852DFD578157D9E4862A8230233F3((MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84*)L_0, L_1, L_2, L_3, NULL);
		return L_4;
	}
}
// Method Definition Index: 42051
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_Free_TisIl2CppFullySharedGenericStruct_mA04A0C63F38A5DB9CAC0294B7DC90B9D35EACC14_fshared (Il2CppFullySharedGenericStruct* ___0_pointer, int64_t ___1_count, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 ___2_label, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	//<source_info:<no-source>:1>
	{
		Il2CppFullySharedGenericStruct* L_0 = ___0_pointer;
		uintptr_t L_1 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		if ((!(((uintptr_t)L_0) == ((uintptr_t)L_1))))
		{
			goto IL_0006;
		}
	}
	{
		return;
	}

IL_0006:
	{
		Il2CppFullySharedGenericStruct* L_2 = ___0_pointer;
		int64_t L_3 = ___1_count;
		int64_t L_4 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 L_5 = ___2_label;
		Il2CppFullySharedGenericStruct* L_6;
		L_6 = ((  Il2CppFullySharedGenericStruct* (*) (Il2CppFullySharedGenericStruct*, int64_t, int64_t, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))(L_2, L_3, L_4, L_5, il2cpp_rgctx_method(method->rgctx_data, 0));
		return;
	}
}
// Method Definition Index: 42050
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_Free_TisIl2CppFullySharedGenericStruct_m6AA7597FA98F8020ABEFACEA0A1D00A384716627_fshared (Il2CppFullySharedGenericStruct* ___0_pointer, int64_t ___1_count, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___2_allocator, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	//<source_info:<no-source>:1>
	{
		Il2CppFullySharedGenericStruct* L_0 = ___0_pointer;
		uintptr_t L_1 = (il2cpp_codegen_conv<uintptr_t,int32_t,int32_t,false,false>(0,NULL));
		if ((!(((uintptr_t)L_0) == ((uintptr_t)L_1))))
		{
			goto IL_0006;
		}
	}
	{
		return;
	}

IL_0006:
	{
		Il2CppFullySharedGenericStruct* L_2 = ___0_pointer;
		int64_t L_3 = ___1_count;
		int64_t L_4 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(0,NULL));
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_5 = ___2_allocator;
		Il2CppFullySharedGenericStruct* L_6;
		L_6 = ((  Il2CppFullySharedGenericStruct* (*) (Il2CppFullySharedGenericStruct*, int64_t, int64_t, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 0)))(L_2, L_3, L_4, L_5, il2cpp_rgctx_method(method->rgctx_data, 0));
		return;
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4* Array_Resize_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_mAB4D8147B48337A05C319FEA2C8645B0DF7CC711 (HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m0593078BB9DC842E2FE2C88E802B606CDC972402_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B* Array_Resize_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m3411CCE45309DF743A524C406531532C4FC8CD72 (HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m9D33FAB445EBAAEAB91BED2D158B3290B610D47E_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258* Array_Resize_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_m87C047C9A0252B77CBF08B5DDEF1D493A14DD276 (UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_m796E3C1D5F3A55A9EFFD1AA98C09AB788198C7A1_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD* Array_Resize_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mFF71C2DBFEFF6594159B5324E0EC19D878450E2E (UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_m70E5F2C375F3B4039114929EBDF86DFCBF1EF99C_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515* Array_Resize_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_mCFE5759D51678924C8C9E90B0A2ABA0AEF5AB6F6 (UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m756E0E44BD2515C8949896575600531B61F5B16B_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6* Array_Resize_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_mCA2193874571CACB1B8D75FBD095B8AD29C6403A (UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m219DC3BB1EF8E9E18A66470555A9974E2C7D9EC6_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6*)(L_7);
	}
}
// Method Definition Index: 42047
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* Array_Resize_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_m68DAEAFD02AE0CA6A976BF0B8B0CAE855D168F32 (uint8_t* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 ___3_label, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint8_t* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 L_3 = ___3_label;
		uint32_t L_4 = sizeof(uint8_t);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mB152D3C658FB48DEDB6E662EEB0C91018E7F2857_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mA60C117B1C01C122EB2D509DD4EADE4FB44C2150((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (uint8_t*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* Array_Resize_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_m1767423622567D1059537F40EF99A5522126FBD0 (uint8_t* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint8_t* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(uint8_t);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mB152D3C658FB48DEDB6E662EEB0C91018E7F2857_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (uint8_t*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t* Array_Resize_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mEE512F558B8B16B19497E4DF8445518F289534BE (int32_t* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(int32_t);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m274F833CD001C63E56A22D8F42C76CE8C6CC39DF_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (int32_t*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t* Array_Resize_TisIntPtr_t_mE1CA5D0262B2B8D4C9453D51DF34A94A0CF3556F (intptr_t* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		intptr_t* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(intptr_t);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisIntPtr_t_mB17C6C168D8CA088879EDD0D5B9923F70F5269B8_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (intptr_t*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint64_t* Array_Resize_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_mD3BD6186C5C7D5B3952897C2602B7C5BEA1A7E81 (uint64_t* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint64_t* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(uint64_t);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m8127223B00E08567EC09EF1D992F89C74F248AB0_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (uint64_t*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4* Array_Resize_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m61718F1D9CCF897E5BE50BB484C8D816C988DF6F (UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m56B5B812224539B0C633BABE891ADDE1973580F6_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926* Array_Resize_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_m1576AD06CC03F6E9D06FC13A0F280519B1666726 (UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mC1EBD5161294EF95B8AC378DDF616869D287DC36_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C* Array_Resize_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_mF6857EF793C9156A611EAA1E63A02352EFE46784 (UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m3DF404928C7038CCD5C05BBBA41C4B2FD35A4253_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2* Array_Resize_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m1082D56F32F50ECA3CFFDB87DB3512F2CE7544F4 (UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_mD84C2AFFB4ED036B913CEAD2FBF8A60E9C3A017D_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67* Array_Resize_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m520F16927035702AB0DF9C30EE88686BDC346F21 (UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m5892DF1B875736B0E3612716E560A4E4AD5F6591_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67*)(L_7);
	}
}
// Method Definition Index: 42047
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Array_Resize_TisIl2CppFullySharedGenericStruct_mDAEE7836D30B1E69218DA92D10DEFB59679D311A_fshared (Il2CppFullySharedGenericStruct* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 ___3_label, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t463F46A118A7AA284DE9A95CCBE473111EF5DF8B = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	{
		Il2CppFullySharedGenericStruct* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		MemoryLabel_t29CE7AB312D2ED888B444BBE5D452F7132EC9DB2 L_3 = ___3_label;
		uint32_t L_4 = SizeOf_T_t463F46A118A7AA284DE9A95CCBE473111EF5DF8B;
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = ((  int32_t (*) (const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))(il2cpp_rgctx_method(method->rgctx_data, 1));
		void* L_7;
		L_7 = Array_Resize_mA60C117B1C01C122EB2D509DD4EADE4FB44C2150((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (Il2CppFullySharedGenericStruct*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppFullySharedGenericStruct* Array_Resize_TisIl2CppFullySharedGenericStruct_m07039200651D43DBB7FBC0CB244761DA34C164FD_fshared (Il2CppFullySharedGenericStruct* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	il2cpp_rgctx_method_init(method);
	const uint32_t SizeOf_T_t4439E68F64AF98A1F1E70993ACB3E0626E7A7954 = il2cpp_codegen_sizeof(il2cpp_rgctx_data(method->rgctx_data, 0));
	//<source_info:<no-source>:1>
	{
		Il2CppFullySharedGenericStruct* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = SizeOf_T_t4439E68F64AF98A1F1E70993ACB3E0626E7A7954;
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = ((  int32_t (*) (const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(method->rgctx_data, 1)))(il2cpp_rgctx_method(method->rgctx_data, 1));
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (Il2CppFullySharedGenericStruct*)(L_7);
	}
}
// Method Definition Index: 42046
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* Array_Resize_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m66D39A0700C852DFD578157D9E4862A8230233F3 (MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* ___0_oldPointer, int64_t ___1_oldCount, int64_t ___2_newCount, AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 ___3_allocator, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84* L_0 = ___0_oldPointer;
		int64_t L_1 = ___1_oldCount;
		int64_t L_2 = ___2_newCount;
		AllocatorHandle_t3CA09720B1F89F91A8DDBA95E74C28A1EC3E3148 L_3 = ___3_allocator;
		uint32_t L_4 = sizeof(MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84);
		int64_t L_5 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_4,NULL));
		int32_t L_6;
		L_6 = UnsafeUtility_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m6AD8B4EF31CDD94CA734378B1443806DAFE68A05_inline(NULL);
		void* L_7;
		L_7 = Array_Resize_mC7BE2965DE3FCF4014D43B606D94951480A65380((void*)L_0, L_1, L_2, L_3, L_5, L_6, NULL);
		return (MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84*)(L_7);
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m274F833CD001C63E56A22D8F42C76CE8C6CC39DF_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mB5611745B3ED473868EF6B61479E2860BBB2223C_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31815
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mFBA66C06ECEB0A2BC1AAE634B6E6BB436F957084_inline (void* ___0_source, int32_t ___1_index, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		void* L_0 = ___0_source;
		int32_t L_1 = ___1_index;
		int32_t L_2;
		L_2 = UnsafeUtilityInternal_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m0B7B877C8F00DCC1C95D7124B3B0D83C6D492345_inline(L_0, L_1, NULL);
		return L_2;
	}
}
// Method Definition Index: 31813
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtility_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mD9A1240B1BCA4029B93CBDCE0196491FEC192E38_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		void* L_0 = ___0_ptr;
		int32_t* L_1 = ___1_output;
		UnsafeUtilityInternal_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mECC3B0123B8C95AF837F45785072BE5466D989C9_inline(L_0, L_1, NULL);
		return;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m0593078BB9DC842E2FE2C88E802B606CDC972402_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m32D4BAEAEAB8B63CD58530C3821D9E05084CFD66_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m9D33FAB445EBAAEAB91BED2D158B3290B610D47E_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m5DB7C8854924D1E1D01E14FB83112EBF8905533C_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_m796E3C1D5F3A55A9EFFD1AA98C09AB788198C7A1_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mD30E57AEFF1E2218A4DCE4EA77B9AE170B7A6BB3_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_m70E5F2C375F3B4039114929EBDF86DFCBF1EF99C_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mB3311E15857CF490132959FAA501B173670237CA_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m756E0E44BD2515C8949896575600531B61F5B16B_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m00C87F308E8C18DB5D47FA0A4A1E8D5738894086_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m219DC3BB1EF8E9E18A66470555A9974E2C7D9EC6_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_mB76E5D04768F570DF3F45ABEB65D6BAFDFFF638A_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mB152D3C658FB48DEDB6E662EEB0C91018E7F2857_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mC44B74F48735FA62BAE3FBA763123E932B9EF822_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisIntPtr_t_mB17C6C168D8CA088879EDD0D5B9923F70F5269B8_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisIntPtr_t_m7FF5CB3788B437CA624174389D25EE9BCB2B212E_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m8127223B00E08567EC09EF1D992F89C74F248AB0_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m29B7D9DA9527706BBA1F73ECEA934CCD0B2D99E3_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m56B5B812224539B0C633BABE891ADDE1973580F6_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_mA359E62F880352C70AB4DC921D67E178E6DACB15_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mC1EBD5161294EF95B8AC378DDF616869D287DC36_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mD2E2A270320B9190D3ECE518E4055D43CFF0C7EC_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m3DF404928C7038CCD5C05BBBA41C4B2FD35A4253_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m73AE74F3CDDD5919B822BA2521D23FF3D3C930E6_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_mD84C2AFFB4ED036B913CEAD2FBF8A60E9C3A017D_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m3CE37249D5247521F44E2D498FDBC3714462A3E3_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m5892DF1B875736B0E3612716E560A4E4AD5F6591_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m78688762B313B8CD8725C345D0D2446E999924E4_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 31821
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtility_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m6AD8B4EF31CDD94CA734378B1443806DAFE68A05_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m844535A4D039E811CA8C8A80A30A2499268FF979_inline(NULL);
		return L_0;
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mB5611745B3ED473868EF6B61479E2860BBB2223C_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t676424B8757032427D7761BC732079DC7512D8AE_mB243DC326B2D814D1A62FDABCC2F2AA541A8C895_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mA24CDBA02971DC06DEDE40B17E72F8C262B6B131_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55746
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_ReadArrayElement_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m0B7B877C8F00DCC1C95D7124B3B0D83C6D492345_inline (void* ___0_source, int32_t ___1_index, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		void* L_0 = ___0_source;
		int32_t L_1 = ___1_index;
		int64_t L_2 = (il2cpp_codegen_conv<int64_t,int32_t,int32_t,false,false>(L_1,NULL));
		uint32_t L_3 = sizeof(int32_t);
		int64_t L_4 = (il2cpp_codegen_conv<int64_t,uint32_t,int32_t,false,false>(L_3,NULL));
		intptr_t L_5 = (il2cpp_codegen_conv<intptr_t,int64_t,int64_t,false,false>(((int64_t)il2cpp_codegen_multiply(L_2, L_4)),NULL));
		int32_t L_6 = (*(int32_t*)((void*)il2cpp_codegen_add((intptr_t)L_0, L_5)));
		return L_6;
	}
}
// Method Definition Index: 55742
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtilityInternal_CopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mECC3B0123B8C95AF837F45785072BE5466D989C9_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		void* L_0 = ___0_ptr;
		int32_t* L_1 = ___1_output;
		UnsafeUtilityInternal_InternalCopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m1ADEAC45E92C10A0C3D9686063C582FDD76F5937_inline(L_0, L_1, NULL);
		return;
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m32D4BAEAEAB8B63CD58530C3821D9E05084CFD66_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t273EAC6E531567E9A44C7A35B5D7DABDD659FC23_m2FCB00D512044D1C5DB62954E2684D67CB44E6C3_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m4AEA1F2D0EBC5D0A1C2819C02415E77F093D96E2_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m5DB7C8854924D1E1D01E14FB83112EBF8905533C_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t245EB4127E14738DE7A1928AA9BD796398AB3E50_m7E4E66C593913FCDFC8C03C06EFC4FFDC9A96BCF_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m0E37BE54FFDD9D93D5ADCA4464466720A0534F51_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mD30E57AEFF1E2218A4DCE4EA77B9AE170B7A6BB3_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7423A7512EC617A3595A4C8B8D8B41727DCB2163_m4D97EFDBFE98987E636ECEAD022BD99CAB158A50_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mB7E3DB3D712EE116DD131EEFE9548C1E0FA09AF9_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mB3311E15857CF490132959FAA501B173670237CA_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7A6E7D9FBDB65F6D78B7607E4870FA0A1580A4D8_m7D90BECD86875EEF05E1DAAF5AD5FDBA356223E1_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mD5CCCB7294CE98231752303CB481A5D7B11E89E6_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_m00C87F308E8C18DB5D47FA0A4A1E8D5738894086_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3E9E85B92672E4CC618A0D4B574EDF6FD77224BE_m74FE4A857995F87CEB1B9C21DC301B1AC06DF227_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_mA7825F57E8BE201BAAE0B00BAA31C1A79D6140D6_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_mB76E5D04768F570DF3F45ABEB65D6BAFDFFF638A_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tC0471DA3EB3B5C4A559BBA96E11DC778D51C2693_m28A0205203C49D5369496BB8258836A6E90B4275_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m6C73B80F6290ED267404794EB2AC27F5EEDB4EF3_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_mC44B74F48735FA62BAE3FBA763123E932B9EF822_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tAF795AF837E3C37401B92C7483D74A4E5145DB4D_m8CB43E8118CC0E65E6E8130381DEB11701F21848_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_m485E79FA6794AD30B29C6414A85A867DF156E5E1_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisIntPtr_t_m7FF5CB3788B437CA624174389D25EE9BCB2B212E_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t1C2506D8F2F53BF0A585917F05CD8BC880EBB752_m6197C4317CE4E01732509D53E08172FEA65ED04E_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisIntPtr_t_mF9218FE174B91F132731D032A744AFC048AFB932_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_m29B7D9DA9527706BBA1F73ECEA934CCD0B2D99E3_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE21040F72E90527C319FF9B07B398E23DF95BE2E_m2D3BC72BBF45866A72C7ADF31F40AEFC56BBD498_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_mAA2B8333B77A7410376516B6E13FCC02AEE6A5AC_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_mA359E62F880352C70AB4DC921D67E178E6DACB15_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tDBD984B5788DF4AA3E90F8C09B5859ED6CB5D981_m0E4242520596E02B9AAE81652FFB8EE3542259AA_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m9B75EBBD9DF8560D30B3C2E1E2C38BF7D85CAF9D_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_mD2E2A270320B9190D3ECE518E4055D43CFF0C7EC_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tB6C5A6C6C3F87E6E89F1D273846EEC6B31DFCDA9_m98AA8DA7813F89F461C97A6373493AB6D70B858B_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_m3C94249F648A50E2D27EDAD82BD9BFD652086097_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m73AE74F3CDDD5919B822BA2521D23FF3D3C930E6_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t71DBCA88B2EAABDA2DD3387E09C204E5E9B12EEA_m05924C78C0CB3BE4A7F3029ACEA5F044E6E559A0_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m4443406CB5E8C38BA71CE11444CF35BD26797053_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m3CE37249D5247521F44E2D498FDBC3714462A3E3_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE427A888F6FDAAB27509B03069802920EC4AA0A3_mE4962E35BBA3FE3704E13AB652950BF09D05115F_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m9D71081BBF4C1B8CCA23832463B32C5362F17249_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_m78688762B313B8CD8725C345D0D2446E999924E4_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t89E0918ECFFC8A376F89EAE40DA2D8C58E85432E_mFD07D65557C1FB04588D27C6A200F7746663E0B0_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_mA5DC0B2032262984D97EFF4CF894E776A34A181F_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55752
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_AlignOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m844535A4D039E811CA8C8A80A30A2499268FF979_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t L_0;
		L_0 = UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3AC3A908E2A7B1D4167756986BD005C60ACF4BC9_m0307FE22A5AF62C62045020268823FBD9652FA71_inline(NULL);
		int32_t L_1;
		L_1 = UnsafeUtilityInternal_SizeOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m64096CFB58B4B0A976A42BC41901B89806B85F69_inline(NULL);
		return ((int32_t)il2cpp_codegen_subtract(L_0, L_1));
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t676424B8757032427D7761BC732079DC7512D8AE_mB243DC326B2D814D1A62FDABCC2F2AA541A8C895_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t676424B8757032427D7761BC732079DC7512D8AE);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_mA24CDBA02971DC06DEDE40B17E72F8C262B6B131_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(int32_t);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55743
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void UnsafeUtilityInternal_InternalCopyPtrToStructure_TisInt32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_m1ADEAC45E92C10A0C3D9686063C582FDD76F5937_inline (void* ___0_ptr, int32_t* ___1_output, const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		int32_t* L_0 = ___1_output;
		void* L_1 = ___0_ptr;
		int32_t L_2 = (*(int32_t*)L_1);
		*(int32_t*)L_0 = L_2;
		return;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t273EAC6E531567E9A44C7A35B5D7DABDD659FC23_m2FCB00D512044D1C5DB62954E2684D67CB44E6C3_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t273EAC6E531567E9A44C7A35B5D7DABDD659FC23);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4_m4AEA1F2D0EBC5D0A1C2819C02415E77F093D96E2_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(HashMapHelper_1_t02163850B336A3F6BBE330638EECFA3CBFCB8CC4);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t245EB4127E14738DE7A1928AA9BD796398AB3E50_m7E4E66C593913FCDFC8C03C06EFC4FFDC9A96BCF_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t245EB4127E14738DE7A1928AA9BD796398AB3E50);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisHashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B_m0E37BE54FFDD9D93D5ADCA4464466720A0534F51_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(HashMapHelper_1_t51F726D16732CD16EFBBB3DE324B481A05388D2B);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7423A7512EC617A3595A4C8B8D8B41727DCB2163_m4D97EFDBFE98987E636ECEAD022BD99CAB158A50_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t7423A7512EC617A3595A4C8B8D8B41727DCB2163);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258_mB7E3DB3D712EE116DD131EEFE9548C1E0FA09AF9_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeQueue_1_t0323897433D8F18174111DB978F6D9EB951CB258);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t7A6E7D9FBDB65F6D78B7607E4870FA0A1580A4D8_m7D90BECD86875EEF05E1DAAF5AD5FDBA356223E1_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t7A6E7D9FBDB65F6D78B7607E4870FA0A1580A4D8);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD_mD5CCCB7294CE98231752303CB481A5D7B11E89E6_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeQueue_1_t0F79CBE5DEA60DE3AE33082C3F7F324FE9066ECD);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3E9E85B92672E4CC618A0D4B574EDF6FD77224BE_m74FE4A857995F87CEB1B9C21DC301B1AC06DF227_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t3E9E85B92672E4CC618A0D4B574EDF6FD77224BE);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515_mA7825F57E8BE201BAAE0B00BAA31C1A79D6140D6_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeQueue_1_t927BA8F9B687AB3656D5A6FFCD1AEE5BE72A5515);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tC0471DA3EB3B5C4A559BBA96E11DC778D51C2693_m28A0205203C49D5369496BB8258836A6E90B4275_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_tC0471DA3EB3B5C4A559BBA96E11DC778D51C2693);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6_m6C73B80F6290ED267404794EB2AC27F5EEDB4EF3_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeRingQueue_1_tAF3191C06795E5D904223D7CA3372A4D56B5B5B6);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tAF795AF837E3C37401B92C7483D74A4E5145DB4D_m8CB43E8118CC0E65E6E8130381DEB11701F21848_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_tAF795AF837E3C37401B92C7483D74A4E5145DB4D);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisByte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_m485E79FA6794AD30B29C6414A85A867DF156E5E1_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(uint8_t);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t1C2506D8F2F53BF0A585917F05CD8BC880EBB752_m6197C4317CE4E01732509D53E08172FEA65ED04E_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t1C2506D8F2F53BF0A585917F05CD8BC880EBB752);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisIntPtr_t_mF9218FE174B91F132731D032A744AFC048AFB932_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(intptr_t);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE21040F72E90527C319FF9B07B398E23DF95BE2E_m2D3BC72BBF45866A72C7ADF31F40AEFC56BBD498_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_tE21040F72E90527C319FF9B07B398E23DF95BE2E);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUInt64_t8F12534CC8FC4B5860F2A2CD1EE79D322E7A41AF_mAA2B8333B77A7410376516B6E13FCC02AEE6A5AC_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(uint64_t);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tDBD984B5788DF4AA3E90F8C09B5859ED6CB5D981_m0E4242520596E02B9AAE81652FFB8EE3542259AA_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_tDBD984B5788DF4AA3E90F8C09B5859ED6CB5D981);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4_m9B75EBBD9DF8560D30B3C2E1E2C38BF7D85CAF9D_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeBitArray_t74DFECCCA980372B6F29B9AA508ACC77A3D6B8D4);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tB6C5A6C6C3F87E6E89F1D273846EEC6B31DFCDA9_m98AA8DA7813F89F461C97A6373493AB6D70B858B_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_tB6C5A6C6C3F87E6E89F1D273846EEC6B31DFCDA9);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926_m3C94249F648A50E2D27EDAD82BD9BFD652086097_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeParallelHashMapData_t43CAB3170FBB624A9CCB6F30C0EC1BB820D57926);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t71DBCA88B2EAABDA2DD3387E09C204E5E9B12EEA_m05924C78C0CB3BE4A7F3029ACEA5F044E6E559A0_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t71DBCA88B2EAABDA2DD3387E09C204E5E9B12EEA);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C_m4443406CB5E8C38BA71CE11444CF35BD26797053_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeQueueBlockHeader_t93BDD5BE3C4A0486BE2A07B68862CFF1F5870D9C);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_tE427A888F6FDAAB27509B03069802920EC4AA0A3_mE4962E35BBA3FE3704E13AB652950BF09D05115F_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_tE427A888F6FDAAB27509B03069802920EC4AA0A3);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2_m9D71081BBF4C1B8CCA23832463B32C5362F17249_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeQueueData_t43073ABB3262F6905C43F9180FCBB71E301597C2);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t89E0918ECFFC8A376F89EAE40DA2D8C58E85432E_mFD07D65557C1FB04588D27C6A200F7746663E0B0_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t89E0918ECFFC8A376F89EAE40DA2D8C58E85432E);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisUnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67_mA5DC0B2032262984D97EFF4CF894E776A34A181F_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(UnsafeText_t93F5D82C1FF7AB12B0E621B9D0EC9855D005FF67);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisAlignOfHelper_1_t3AC3A908E2A7B1D4167756986BD005C60ACF4BC9_m0307FE22A5AF62C62045020268823FBD9652FA71_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(AlignOfHelper_1_t3AC3A908E2A7B1D4167756986BD005C60ACF4BC9);
		return (int32_t)L_0;
	}
}
// Method Definition Index: 55751
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t UnsafeUtilityInternal_SizeOf_TisMemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84_m64096CFB58B4B0A976A42BC41901B89806B85F69_inline (const RuntimeMethod* method) 
{
	//<source_info:<no-source>:1>
	{
		uint32_t L_0 = sizeof(MemoryBlock_t83C4AD217E04686899AC67FBD8D14A7C9AAC6E84);
		return (int32_t)L_0;
	}
}
