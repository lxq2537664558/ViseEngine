using System;
using System.Collections.Generic;

namespace CSUtility.Support
{
    public enum enResourceType
    {
        Unknow,
        Dll,
        MapFiles,
        MeshTemplate,
        MeshSource,
        SimMeshSource,
        PathMeshSource,
        Socket,
        Material,
        Technique,
        Texture,
        Action,
        Notify,
        Effect,
        UI,
        UVAnim,
        Event,
        FSM,
        Illumination,
        Metadata,
        Shader,
        Template,
        Font,
        Folder,
        Config,
        Sound,
        RoleTemplate,
    }

    // 标注属性的资源类型，用于发布
    public sealed class ResourcePublishAttribute : System.Attribute
    {
        public enResourceType ResourceType;

        public ResourcePublishAttribute(enResourceType resType) 
        {
            ResourceType = resType;
        }
    }

    public class IFileConfig
    {
        static IFileConfig smInstance = new IFileConfig();
        public static IFileConfig Instance
        {
            get { return smInstance; }
        }
        
        public static DateTime ZeusCenturyBegin
        {
            get;
        } = new DateTime(2009, 1, 28);
        
        public static Guid DefaultAI
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("931917dd-45de-4688-a93f-c0a5d8b2cb59");
        
        [DataValueAttribute("MetaDataDirectory")]
        public static string MetaDataDirectory
        {
            get;
            set;
        } = "metadata";	// MetaData路径
        
        [DataValueAttribute("MapDirectory")]
        public static string MapDirectory
        {
            get;
            set;
        } = "Maps";				// 地图路径
        [DataValueAttribute("MapExtension")]
        public static string MapExtension
        {
            get;
            set;
        } = ".map";				// 地图扩展名
        // 这个要删除
        [DataValueAttribute("EditorSourceDirectory")]
        public static string EditorSourceDirectory
        {
            get;
            set;
        } = "EditorSource";	// 编辑器用资源路径
        [DataValueAttribute("MaterialFunctionDirectory")]
        public static string MaterialFunctionDirectory
        {
            get;
            set;
        } = "shader/function";	// 材质附加函数路径
        
        [DataValueAttribute("MaterialExtension")]
        public static string MaterialExtension
        {
            get;
            set;
        } = ".mtl";		// 材质扩展名

        [DataValueAttribute("MaterialTechniqueExtension")]
        public static string MaterialTechniqueExtension
        {
            get;
            set;
        } = ".tech";	// 材质Technique扩展名

        [DataValueAttribute("MeshTemplateExtension")]
        public static string MeshTemplateExtension
        {
            get;
            set;
        } = ".mesh";		// MeshTemplate扩展名

        [DataValueAttribute("UVAnimExtension")]
        public static string UVAnimExtension
        {
            get;
            set;
        } = ".uvanim";				// UVAnim扩展名
        [DataValueAttribute("UVAnimDefaultTechnique")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static Guid UVAnimDefaultTechnique
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("f5335399-627b-472f-94c0-37e172f7ec12");

        [DataValueAttribute("RoleTemplateExtension")]
        public static string RoleTemplateExtension
        {
            get;
            set;
        } = ".role";		// RoleTemplate扩展名
        
        [DataValueAttribute("ItemPayTemplateExtension")]
        public static string ItemPayTemplateExtension
        {
            get;
            set;
        } = ".payitem";		// RoleTemplate扩展名
        
        [DataValueAttribute("NpcComAttExtension")]
        public static string NpcComAttExtension
        {
            get;
            set;
        } = ".npcAtt";		// npccommonAttribute扩展名

        [DataValueAttribute("NpcTransfarExtension")]
        public static string NpcTransfarExtension
        {
            get;
            set;
        } = ".npcTrans";		// npctransfar扩展名

        [DataValueAttribute("NpcNameAffixExtension ")]
        public static string NpcNameAffixExtension
        {
            get;
            set;
        } = ".nnafx";		// npctransfar扩展名

        [DataValueAttribute("NpcNewGuideExtension ")]
        public static string NpcNewGuideExtension
        {
            get;
            set;
        } = ".duideS";		//新手指引扩展名
        
        [DataValueAttribute("NpcDynamicExtension")]
        public static string NpcDynamicExtension
        {
            get;
            set;
        } = ".dynpc";		// 动态生成npcs扩展名

        [DataValueAttribute("StoreShopExtension")]
        public static string StoreShopExtension
        {
            get;
            set;
        } = ".shop";		// 商店物品扩展名

        [DataValueAttribute("NpcAddExtension")]
        public static string NpcAddExtension
        {
            get;
            set;
        } = ".npcadd";		// NpcAdd扩展名

        [DataValueAttribute("ReValueExtension")]
        public static string ReValueExtension
        {
            get;
            set;
        } = ".npcadd";		// ReduceValue扩展名

        [DataValueAttribute("RoleCommonTemplateExtension")]
        public static string RoleCommonTemplateExtension
        {
            get;
            set;
        } = ".roleCom";		// RoleTemplate扩展名

        [DataValueAttribute("ActionExtension")]
        public static string ActionExtension
        {
            get;
            set;
        } = ".vma";				// Action扩展名

        [DataValueAttribute("ActionNotifyExtension")]
        public static string ActionNotifyExtension
        {
            get;
            set;
        } = ".van";		// ActionNotify扩展名

        [DataValueAttribute("ParticleExtension")]
        public static string ParticleExtension
        {
            get;
            set;
        } = ".ptc";			// Particle扩展名

        public static string EffectExtension
        {
            get;
            set;
        } = ".eft";       // Effect扩展名

        [DataValueAttribute("IlluminationExtension")]
        public static string IlluminationExtension
        {
            get;
            set;
        } = ".ilu";		// Illumination扩展名

        [DataValueAttribute("MeshSourceExtension")]
        public static string MeshSourceExtension
        {
            get;
            set;
        } = ".vms";			// MeshSource扩展名

        [DataValueAttribute("PathMeshExtension")]
        public static string PathMeshExtension
        {
            get;
            set;
        } = ".path";			// PathMesh扩展名

        [DataValueAttribute("SimpleMeshExtension")]
        public static string SimpleMeshExtension
        {
            get;
            set;
        } = ".sim";			// SimMesh扩展名
        
        [DataValueAttribute("MeshSocketExtension")]
        public static string MeshSocketExtension
        {
            get;
            set;
        } = ".socket";
        
        [DataValueAttribute("PhysicGeometryExtension")]
        public static string PhysicGeometryExtension
        {
            get;
            set;
        } = ".phy";
        
        [DataValueAttribute("RuneTemplateExtension")]
        public static string RuneTemplateExtension
        {
            get;
            set;
        } = ".rune";

        // 默认资源路径
        [DataValueAttribute("DefaultResourceDirectory")]
        public static string DefaultResourceDirectory
        {
            get;
            set;
        } = "resources";
        
        [DataValueAttribute("DefaultShaderDirectory")]
        public static string DefaultShaderDirectory
        {
            get;
            set;
        } = "shader";
        
        [DataValueAttribute("DefaultShaderCacheDirectory")]
        public static string DefaultShaderCacheDirectory
        {
            get;
            set;
        } = "shader/cachebin";
        
        //[DataValueAttribute("DefaultMaterialDirectory")]
        //public static string DefaultMaterialDirectory
        //{
        //    get;
        //    set;
        //} = "Shader/Material";		// 默认材质路径

        [DataValueAttribute("DefaultTextureFile")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string DefaultTextureFile
        {
            get;
            set;
        } = "resources/default/uvchecker.png";		// 默认贴图文件名

        [DataValueAttribute("DefaultUIDirectory")]
        public static string DefaultUIDirectory
        {
            get;
            set;
        } = "UI";		// 默认UI路径

        [DataValueAttribute("DefaultUVAnimDirectory")]
        public static string DefaultUVAnimDirectory
        {
            get;
            set;
        } = "UI/UVAnim";	// 默认UVAnim路径

        [DataValueAttribute("DefaultUITemplateDirectory")]
        public static string DefaultUITemplateDirectory
        {
            get;
            set;
        } = "UI/Template";	// 默认UI模板路径

        [DataValueAttribute("DefaultUIRectangleMaterial")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static Guid DefaultUIRectangleMaterial
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("036b8663-3693-48e4-bbfc-2e00a8e4dd8f");  // 默认界面绘制矩形使用的材质

        [DataValueAttribute("DefaultFontDirectory")]
        public static string DefaultFontDirectory
        {
            get;
            set;
        } = "Font";		// 默认Font文件路径

        [DataValueAttribute("DefaultFSMDirectory")]
        public static string DefaultFSMDirectory
        {
            get;
            set;
        } = "FSM";		// 默认FSM路径

        [DataValueAttribute("FSMDlls_Client_Directory")]
        public static string FSMDlls_Client_Directory
        {
            get;
            set;
        } = "binary/FSM";	// 客户端FSM dll路径

        [DataValueAttribute("FSMDlls_Server_Directory")]
        public static string FSMDlls_Server_Directory
        {
            get;
            set;
        } = "server/FSM";	// 服务器端FSM dll路径

        [DataValueAttribute("DefaultActionFile")]
        [ResourcePublishAttribute(enResourceType.Action)]
        public static string DefaultActionFile
        {
            get;
            set;
        } = "resources/default/walk.vma";	// 默认Action路径

        [DataValueAttribute("DefaultMeshDirectory")]
        public static string DefaultMeshDirectory
        {
            get;
            set;
        } = "Mesh";		// 默认Mesh路径

        [DataValueAttribute("DefaultTextureDirectory")]
        public static string DefaultTextureDirectory
        {
            get;
            set;
        } = "Texture";	// 默认Texture路径

        [DataValueAttribute("DefaultSoundDirectory")]
        public static string DefaultSoundDirectory
        {
            get;
            set;
        } = "Sound";    // 默认声音资源路径

        [DataValueAttribute("DefaultEffectDirectory")]
        public static string DefaultEffectDirectory
        {
            get;
            set;
        } = "Effect";	// 默认特效（粒子）路径

        [DataValueAttribute("DefaultMeshTemplateDirectory")]
        public static string DefaultMeshTemplateDirectory
        {
            get;
            set;
        } = "MeshTemplate";	// 默认MeshTemplate路径
        
        [DataValueAttribute("DefaultNewGuideDirectory")]
        public static string DefaultNewGuideDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/NewGuide";	// 默认npc名称路径

        [DataValueAttribute("DefaultRoleTemplateDirectory")]
        public static string DefaultRoleTemplateDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/Role";	// 默认RoleTemplate路径
        
        [DataValueAttribute("ItemPayTemplateDirectory")]
        public static string ItemPayTemplateDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/PayItem";	// 默认RoleTemplate路径

        [DataValueAttribute("DefaultNpcComAttDirectory")]
        public static string DefaultNpcComAttDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/NpcComAtt";	// 默认NpcComAtt路径

        [DataValueAttribute("DefaultNpcTransfarDirectory")]
        public static string DefaultNPCNameAffixDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/NPCNameAffix";	// 默认npc名称路径

        [DataValueAttribute("DefaultNpcTransfarDirectory")]
        public static string DefaultNpcTransfarDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/GroupTransfar";	// 默认传送模板路径

        [DataValueAttribute("DefaultDynamicNpcDirectory")]
        public static string DefaultDynamicNpcDirectory
        {
            get;
            set;
        } = "ZeusGame/Template/DynamicNpc"; // 默认动态生成npc列表的模板路径
        static string mDefaultStoreDirectory = "ZeusGame/Template/Store";	// 默认动态生成npc列表的模板路径
        [DataValueAttribute("DefaultStoreDirectory")]
        public static string DefaultStoreDirectory
        {
            get { return mDefaultStoreDirectory; }
            set { mDefaultStoreDirectory = value; }
        }
        static string mDefaultNpcAddDirectory = "ZeusGame/Template/NpcAdd";	// 默认NpcAdd路径
        [DataValueAttribute("DefaultNpcAddDirectory")]
        public static string DefaultNpcAddDirectory
        {
            get { return mDefaultNpcAddDirectory; }
            set { mDefaultNpcAddDirectory = value; }
        }
        static string mDefaultReduceValDirectory = "ZeusGame/Template/ReduceValue";	// 默认ReduceValue路径
        [DataValueAttribute("DefaultReduceValDirectory")]
        public static string DefaultReduceValDirectory
        {
            get { return mDefaultReduceValDirectory; }
            set { mDefaultReduceValDirectory = value; }
        }
        static string mDefaultRoleCommonTemplateDirectory = "ZeusGame/Template/CommonRole";	// 默认RoleCommonTemplate路径
        [DataValueAttribute("DefaultRoleCommonTemplateDirectory")]
        public static string DefaultRoleCommonTemplateDirectory
        {
            get { return mDefaultRoleCommonTemplateDirectory; }
            set { mDefaultRoleCommonTemplateDirectory = value; }
        }
        static string mDefaultSkillTemplateDirectory = "Template/Skill";	// 默认SkillTemplate路径
        [DataValueAttribute("DefaultSkillTemplateDirectory")]
        public static string DefaultSkillTemplateDirectory
        {
            get { return mDefaultSkillTemplateDirectory; }
            set { mDefaultSkillTemplateDirectory = value; }
        }
        static string mDefaultRelifePointTemplateDirectory = "ZeusGame/Template/RelifePoint";    //默认RelifePointTemplate路径
        [DataValueAttribute("DefaultRelifePointTemplateDirectory")]
        public static string DefaultRelifePointTemplateDirectory
        {
            get { return mDefaultRelifePointTemplateDirectory; }
            set { mDefaultRelifePointTemplateDirectory = value; }
        }

        static string mDefaultBuffTemplateDirectory = "MobaGame/Release/resources/Buff";	// 默认BuffTemplate路径
        [DataValueAttribute("DefaultBuffTemplateDirectory")]
        public static string DefaultBuffTemplateDirectory
        {
            get { return mDefaultBuffTemplateDirectory; }
            set { mDefaultBuffTemplateDirectory = value; }
        }
        static string mDefaultCareerTemplateDirectory = "ZeusGame/Template/Career";	// 默认CareerTemplate路径
        [DataValueAttribute("DefaultCareerTemplateDirectory")]
        public static string DefaultCareerTemplateDirectory
        {
            get { return mDefaultCareerTemplateDirectory; }
            set { mDefaultCareerTemplateDirectory = value; }
        }
        static string mDefaultPerformOptionsTemplateDirectory = "ZeusGame/Template/PerformOptions";	// 默认CareerTemplate路径
        [DataValueAttribute("DefaultPerformOptionsTemplateDirectory")]
        public static string DefaultPerformOptionsTemplateDirectory
        {
            get { return mDefaultPerformOptionsTemplateDirectory; }
            set { mDefaultPerformOptionsTemplateDirectory = value; }
        }
        static string mDefaultRuneTemplateDirectory = "ZeusGame/Template/Rune";	// 默认RuneTemplate路径
        [DataValueAttribute("DefaultRuneTemplateDirectory")]
        public static string DefaultRuneTemplateDirectory
        {
            get { return mDefaultRuneTemplateDirectory; }
            set { mDefaultRuneTemplateDirectory = value; }
        }
        static string mDefaultDropTemplateDirectory = "ZeusGame/Template/Drop";	// 默认DropTemplate路径
        [DataValueAttribute("DefaultDropTemplateDirectory")]
        public static string DefaultDropTemplateDirectory
        {
            get { return mDefaultDropTemplateDirectory; }
            set { mDefaultDropTemplateDirectory = value; }
        }
        static string mDefaultTaskTemplateDirectory = "ZeusGame/Template/Task";	// 默认TaskTemplate路径
        [DataValueAttribute("DefaultTaskTemplateDirectory")]
        public static string DefaultTaskTemplateDirectory
        {
            get { return mDefaultTaskTemplateDirectory; }
            set { mDefaultTaskTemplateDirectory = value; }
        }
        static string mDefaultRandTaskTemplateDirectory = "ZeusGame/Template/RandTask";	// 默认randTaskTemplate路径
        [DataValueAttribute("DefaultTaskTemplateDirectory")]
        public static string DefaultRandTaskTemplateDirectory
        {
            get { return mDefaultRandTaskTemplateDirectory; }
            set { mDefaultRandTaskTemplateDirectory = value; }
        }
        static string mDefaultTalkTemplateDirectory = "ZeusGame/Template/Talk";	// 默认TalkTemplate路径
        [DataValueAttribute("DefaultTalkTemplateDirectory")]
        public static string DefaultTalkTemplateDirectory
        {
            get { return mDefaultTalkTemplateDirectory; }
            set { mDefaultTalkTemplateDirectory = value; }
        }
        //string mDefaultEffectTemplateDirectory = "ZeusGame/Template/Effect";	// 默认EffectTemplate路径
        //[DataValueAttribute("DefaultEffectTemplateDirectory")]
        //public string DefaultEffectTemplateDirectory
        //{
        //    get { return mDefaultEffectTemplateDirectory; }
        //    set { mDefaultEffectTemplateDirectory = value; }
        //}

        // 默认Event路径
        [DataValueAttribute("DefaultEventDirectory")]
        public static string DefaultEventDirectory
        {
            get;
            set;
        } = "events";	
        // 客户端Event dll路径
        [DataValueAttribute("EventDlls_Client_Directory")]
        public static string EventDlls_Client_Directory
        {
            get;
            set;
        } = "binary/events";
        // 服务器Event dll路径
        [DataValueAttribute("EventDlls_Server_Directory")]
        public static string EventDlls_Server_Directory
        {
            get;
            set;
        } = "server/events";
        // 服务器路径
        [DataValueAttribute("Server_Directory")]
        public static string Server_Directory
        {
            get;
            set;
        } = "server";
        // 客户端路径
        [DataValueAttribute("Server_Directory")]
        public static string Client_Directory
        {
            get;
            set;
        } = "binary";
        // 默认的材质路径表文件名称
        [DataValueAttribute("DefaultMaterialFileDictionaryFile")]
        public static string DefaultMaterialFileDictionaryFile
        {
            get;
            set;
        } = "MatFileDic.dic";
        // 默认材质
        [DataValueAttribute("DefaultMaterialId")]
        [ResourcePublishAttribute(enResourceType.Material)]
        public static System.Guid DefaultMaterialId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("dce1e9e2-8ead-4b6b-a577-4ca03a94c52c");
        //// 引擎系统内部使用的材质路径
        //[DataValueAttribute("SystemMaterialPath")]
        //[ResourcePublishAttribute(enResourceType.Folder)]
        //public static string SystemMaterialPath
        //{
        //    get;
        //    set;
        //} = "shader/Material";
        static System.Guid mDefaultTechniqueId = CSUtility.Support.IHelper.GuidParse("c1da4bfd-d5fb-4094-9d31-838f5eee71d1");					// 默认Tech Id
        [DataValueAttribute("DefaultTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultTechniqueId
        {
            get { return mDefaultTechniqueId; }
            set { mDefaultTechniqueId = value; }
        }
        static System.Guid mDefaultSkeletonMaterialTechniqueId = System.Guid.Empty;	// 默认骨骼材质Tech
        [DataValueAttribute("DefaultSkeletonMaterialTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultSkeletonMaterialTechniqueId
        { 
            get { return mDefaultSkeletonMaterialTechniqueId; }
            set { mDefaultSkeletonMaterialTechniqueId = value; }
        }
        // 默认简化模型Tech
        [DataValueAttribute("DefaultSimplateMeshTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultSimplateMeshTechniqueId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("11a54745-fd7c-4877-ab81-c48769aa5700");

        [DataValueAttribute("DefaultPathMeshTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultPathMeshTechniqueId
        {
            get ;
            set ;
        } = CSUtility.Support.IHelper.GuidParse("3c489c1d-63a5-4f2b-93f5-0605aad9b0fa");			// 默认寻路模型Tech
        // 默认光源简化模型Tech
        [DataValueAttribute("DefaultLightEditorSignMeshTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultLightEditorSignMeshTechniqueId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("7e5bc1d1-5a4c-4b99-8c66-5d54b3a086f3");	
        // 默认光源范围模型Tech
        [DataValueAttribute("DefaultLightEditorRangeMeshTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultLightEditorRangeMeshTechniqueId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("7e5bc1d1-5a4c-4b99-8c66-5d54b3a086f3");
        // 默认Decal简化模型Tech
        [DataValueAttribute("DefaultDecalEditorSignMeshTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultDecalEditorSignMeshTechniqueId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("da50c1d6-b4e2-41b2-a4e3-817aef2f0224");
        // 默认Decal范围模型Tech
        [DataValueAttribute("DefaultDecalEditorRangeMeshTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultDecalEditorRangeMeshTechniqueId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("da50c1d6-b4e2-41b2-a4e3-817aef2f0224");
        [DataValueAttribute("DefaultDecalTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultDecalTechniqueId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("dfd9df4b-9626-4a00-aec1-b32de0a56873");	// 默认Decal Tech
        // 默认寻路Decal Tech
        [DataValueAttribute("DefaultNavigationDecalTechId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultNavigationDecalTechId
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("320364af-c3ca-4ff2-840b-857448a9ecfe");
        static System.Guid mDefaultTriggerTechniqueId = CSUtility.Support.IHelper.GuidParse("cdb15e62-4fb4-4d91-8285-dbd3a3316365");				// 默认Trigger Tech
        [DataValueAttribute("DefaultTriggerTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultTriggerTechniqueId
        {
            get { return mDefaultTriggerTechniqueId; }
            set { mDefaultTriggerTechniqueId = value; }
        }

        // 后期特效触发器贴图
        [DataValueAttribute("DefaultRandomNormalFile")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string DefaultRandomNormalFile
        {
            get ; 
            set ;
        } = "resources/default/texture/Program/RandomNormal.jpg";
        // 后期特效触发器贴图
        [DataValueAttribute("DefaultColorGradeFile")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string DefaultColorGradeFile
        {
            get ; 
            set ; 
        } = "resources/default/texture/Colorgrading/OriginalRGBTable16x1.png";
        //069ec102-d4e9-4c79-8c5b-dec0992d6297
        // 地形笔刷材质
        [DataValueAttribute("TerrainBrushTechnique")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid TerrainBrushTechnique
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("dfd9df4b-9626-4a00-aec1-b32de0a56873");

        // 默认Tile画刷
        [DataValueAttribute("DefaultTileBrushTechnique")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string DefaultTileBrushTechnique
        {
            get;
            set;
        } = "resources/default/Editor/texture/defaultTileBrush.jpg";

        [DataValueAttribute("DefaultRegionTechnique")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid DefaultRegionTechnique
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("0f97c0e7-09ef-4374-80d5-70e15f57d02d");  // 范围默认材质
        //System.Guid mEdgeDetectTechniqueFriend = CSUtility.Support.IHelper.GuidParse("51a85332-bfcb-40f7-9886-ff0668ecc94d");  // 勾边材质
        //[DataValueAttribute("EdgeDetectTechniqueFriend")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueFriend
        //{
        //    get { return mEdgeDetectTechniqueFriend; }
        //    set { mEdgeDetectTechniqueFriend = value; }
        //}
        //System.Guid mEdgeDetectTechniqueNeutral = CSUtility.Support.IHelper.GuidParse("1a009d9d-1f73-4b4b-86f0-5f0d7f3909ef");  // 勾边材质
        //[DataValueAttribute("EdgeDetectTechniqueNeutral")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueNeutral
        //{
        //    get { return mEdgeDetectTechniqueNeutral; }
        //    set { mEdgeDetectTechniqueNeutral = value; }
        //}
        //System.Guid mEdgeDetectTechniqueEnemy = CSUtility.Support.IHelper.GuidParse("3c440953-67cb-4f24-82f0-a46d59ac0ff9");  // 勾边材质
        //[DataValueAttribute("EdgeDetectTechniqueEnemy")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueEnemy
        //{
        //    get { return mEdgeDetectTechniqueEnemy; }
        //    set { mEdgeDetectTechniqueEnemy = value; }
        //}
        //System.Guid mEdgeDetectTechniqueCreateRole = CSUtility.Support.IHelper.GuidParse("4ca33046-a0c9-4554-ae79-e0f6fd379bba");  // 勾边材质
        //[DataValueAttribute("EdgeDetectTechniqueCreateRole")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueCreateRole
        //{
        //    get { return mEdgeDetectTechniqueCreateRole; }
        //    set { mEdgeDetectTechniqueCreateRole = value; }
        //}

        //System.Guid mEdgeDetectTechniqueNPCInitializer = CSUtility.Support.IHelper.GuidParse("adae6530-1007-4de1-850a-a84accdf6ce3");  // 种植NPC勾边材质
        //[DataValueAttribute("EdgeDetectTechniqueNPCInitializer")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueNPCInitializer
        //{
        //    get { return mEdgeDetectTechniqueNPCInitializer; }
        //    set { mEdgeDetectTechniqueNPCInitializer = value; }
        //}
        //System.Guid mEdgeDetectTechniqueGatherNPCInitializer = CSUtility.Support.IHelper.GuidParse("2e7f8302-5b3e-422c-a386-b044f7c783cc");  // 种植采集NPC勾边材质
        //[DataValueAttribute("EdgeDetectTechniqueGatherNPCInitializer")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueGatherNPCInitializer
        //{
        //    get { return mEdgeDetectTechniqueGatherNPCInitializer; }
        //    set { mEdgeDetectTechniqueGatherNPCInitializer = value; }
        //}
        //System.Guid mEdgeDetectTechniqueLight = CSUtility.Support.IHelper.GuidParse("f6372a48-8570-41ca-9029-1001090ebda9");  // 选中灯光时用的材质
        //[DataValueAttribute("EdgeDetectTechniqueLight")]
        //[ResourcePublishAttribute(enResourceType.Technique)]
        //public System.Guid EdgeDetectTechniqueLight
        //{
        //    get { return mEdgeDetectTechniqueLight; }
        //    set { mEdgeDetectTechniqueLight = value; }
        //}

        static System.Guid mShadowMapRenderTechniqueId = CSUtility.Support.IHelper.GuidParse("809b06a0-3a6a-4213-9473-a0cd0844e529");
        [DataValueAttribute("ShadowMapRenderTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid ShadowMapRenderTechniqueId
        {
            get { return mShadowMapRenderTechniqueId; }
            set { mShadowMapRenderTechniqueId = value; }
        }

        static System.Guid mHeadLightShadowMapRenderTechniqueId = CSUtility.Support.IHelper.GuidParse("809b06a0-3a6a-4213-9473-a0cd0844e529");
        [DataValueAttribute("HeadLightShadowMapRenderTechniqueId")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid HeadLightShadowMapRenderTechniqueId
        {
            get { return mHeadLightShadowMapRenderTechniqueId; }
            set { mHeadLightShadowMapRenderTechniqueId = value; }
        }

        [DataValueAttribute("AxisMaterialRed")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialRed
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("ecee7f26-10d0-40c4-a858-64bbc3fc63e4");
        [DataValueAttribute("AxisMaterialGreen")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialGreen
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("88bb3b94-f4dd-487c-9940-525fb94c910a");
        [DataValueAttribute("AxisMaterialBlue")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialBlue
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("d8ea488d-08ad-4c02-a1a7-cbd718f024f6");
        [DataValueAttribute("AxisMaterialWhite")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialWhite
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("4d9c1038-ae18-45d4-84e5-a343c7fdcced");
        [DataValueAttribute("AxisMaterialYellow")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialYellow
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("6a47bb90-3f9d-4bc0-87b2-39e1d5918f86");
        [DataValueAttribute("AxisMaterialAlphaYellow")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialAlphaYellow
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("86128485-edac-48f2-9502-edc164adfba9");
        [DataValueAttribute("AxisMaterialAlpha")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialAlpha
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("dabb3c21-92d5-4614-a05e-9cfed34cf323");
        [DataValueAttribute("AxisMaterialXY")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialXY
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("c3597738-c85e-4b09-835d-347d4d64975f");
        [DataValueAttribute("AxisMaterialXZ")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialXZ
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("528ec8dd-c53a-4460-b577-db1a084dd590");
        [DataValueAttribute("AxisMaterialYZ")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static System.Guid AxisMaterialYZ
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("15f55c55-9281-4546-8080-fa0a53180f0d");

        // 地形笔刷贴图
        [DataValueAttribute("NavigationBrushDecalTextureFile")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string NavigationBrushDecalTextureFile
        {
            get;
            set;
        } = "resources/default/texture/NavBrush.jpg";
        static string mShadowMapSmoothTextureFile = "Texture/Assist/ShadowSmoothTransition.dds";		// Shadowmap边缘半透贴图
        [DataValueAttribute("ShadowMapSmoothTextureFile")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string ShadowMapSmoothTextureFile
        {
            get { return mShadowMapSmoothTextureFile; }
            set { mShadowMapSmoothTextureFile = value; }
        }
        static string mDefaultEmptyTextureFile = "Texture/Assist/emptyDefault.dds";		// 纹理未加载完时使用的临时纹理
        [DataValueAttribute("DefaultEmptyTextureFile")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string DefaultEmptyTextureFile
        {
            get { return mDefaultEmptyTextureFile; }
            set { mDefaultEmptyTextureFile = value; }
        }
        static System.Guid mDefaultMapId = CSUtility.Support.IHelper.GuidParse("f3d00f7a-a75d-4e5c-ab61-ee68af232069");							// 默认地图ID
        [DataValueAttribute("DefaultMapId")]
        public static System.Guid DefaultMapId
        {
            get { return mDefaultMapId; }
            set { mDefaultMapId = value; }
        }

        // 移动坐标轴模型
        [DataValueAttribute("MoveAxisMesh")]
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        public static string MoveAxisMesh
        {
            get;
            set;
        } = "editorresources/mesh/MoveAxis.vms";
        // 旋转坐标轴模型
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("RotAxisMesh")]
        public static string RotAxisMesh
        {
            get;
            set;
        } = "editorresources/mesh/RotAxis.vms";
        // 缩放坐标轴模型
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("ScaleAxisMesh")]
        public static string ScaleAxisMesh
        {
            get;
            set;
        } = "editorresources/mesh/ScaleAxis.vms";	
        // socket模型
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("SocketHandleMesh")]
        public static string SocketHandleMesh
        {
            get;
            set;
        } = "editorresources/mesh/SocketHandle.vms";
        // Socket材质(未选中)
        [ResourcePublishAttribute(enResourceType.Technique)]
        [DataValueAttribute("SocketHandleTechnique_Normal")]
        public static Guid SocketHandleTechnique_Normal
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("8debe658-ec5b-483d-92c0-b829fe5bb896");
        // Socket材质(选中)
        [ResourcePublishAttribute(enResourceType.Technique)]
        [DataValueAttribute("SocketHandleTechnique_Selected")]
        public static Guid SocketHandleTechnique_Selected
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("ea9c8f87-27eb-4dc7-be29-3ca5d27cb204");
        
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("BoxMesh")]
        public static string BoxMesh
        {
            get;
            set;
        } = "editorresources/mesh/box.vms";

        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("CylinderMesh")]
        public static string CylinderMesh
        {
            get;
            set;
        } = "editorresources/mesh/cylinder.vms";

        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("SphereMesh")]
        public static string SphereMesh
        {
            get;
            set;
        } = "editorresources/mesh/sphere.vms";

        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("PlaneMesh")]
        public static string PlaneMesh
        {
            get;
            set;
        } = "editorresources/mesh/plane.vms";

        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("TeapotMesh")]
        public static string TeapotMesh
        {
            get;
            set;
        } = "editorresources/mesh/teapot.vms";
        // 用作比例尺的标准模型
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        [DataValueAttribute("TeapotMesh")]
        public static string StandardMesh
        {
            get;
            set;
        } = "editorresources/mesh/HumanMale.vms";

        static string mDefaultTemplateDir = "Template";				// 默认模板路径
        [DataValueAttribute("DefaultTemplateDir")]
        public static string DefaultTemplateDir
        {
            get { return mDefaultTemplateDir; }
            set { mDefaultTemplateDir = value; }
        }

        // 粒子默认模型
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        [DataValueAttribute("ParticleDefaultMesh")]
        public static Guid ParticleDefaultMesh
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("bd49bb9d-79e0-4db4-831d-353a7b67b805");

        // 巡逻点模型
        static Guid mPatrolPointMesh = CSUtility.Support.IHelper.GuidParse("26fc50cf-efff-4fb6-bf29-f347396d1703");
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        [DataValueAttribute("PatrolPointMesh")]
        public static Guid PatrolPointMesh
        {
            get { return mPatrolPointMesh; }
            set { mPatrolPointMesh = value; }
        }

        // 场景点模型
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        [DataValueAttribute("ScenePointMesh")]
        public static Guid ScenePointMesh
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("3d067190-64c1-4b23-ba08-b6ef7eeb6000");
        // 摄像机模型
        static Guid mCameraMesh = CSUtility.Support.IHelper.GuidParse("9f3858dc-e5c6-4e81-a6c4-d054c9399f92"); 
         [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        [DataValueAttribute("CameraMesh")]
        public static Guid CameraMesh
        {
            get { return mCameraMesh; }
            set { mCameraMesh = value; }
        }
        // 路点模型
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        [DataValueAttribute("NavigationPointMesh")]
        public static Guid NavigationPointMesh
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidParse("3d067190-64c1-4b23-ba08-b6ef7eeb6000");

        static Guid mTextBoxCursor = CSUtility.Support.IHelper.GuidTryParse("ed0015c4-824f-4806-9ac7-3b7b9df89fa0");
        [ResourcePublishAttribute(enResourceType.UVAnim)]
        [DataValueAttribute("TextBoxCursor")]
        public static Guid TextBoxCursor
        {
            get { return mTextBoxCursor; }
            set { mTextBoxCursor = value; }
        }

        static string mDefaultPrefabFolder = "Prefab";
        [DataValueAttribute("DefaultPrefabFolder")]
        public static string DefaultPrefabFolder
        {
            get { return mDefaultPrefabFolder; }
            set { mDefaultPrefabFolder = value; }
        }

        static string mPrefabResExtension = ".pre";
        [DataValueAttribute("PrefabResExtension")]
        public static string PrefabResExtension
        {
            get { return mPrefabResExtension; }
            set { mPrefabResExtension = value; }
        }

        static string mDefaultRandomNormal = "texture/Effect/Program/RandomNormal.jpg";
        [DataValueAttribute("DefaultRandomNormal")]
        [ResourcePublishAttribute(enResourceType.Texture)]
        public static string DefaultRandomNormal
        {
            get { return mDefaultRandomNormal; }
            set { mDefaultRandomNormal = value; }
        }

        static string mDefaultFont = "Font/msyh.ttf";
        [DataValueAttribute("DefaultFont")]
        [ResourcePublishAttribute(enResourceType.Font)]
        public static string DefaultFont
        {
            get { return mDefaultFont; }
            set { mDefaultFont = value; }
        }

        
        [DataValueAttribute("DefaultMesh")]
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        public static string DefaultMesh
        {
            get;
            set;
        } = "resources/default/editor/mesh/box.vms";

        [DataValueAttribute("DefaultMesh")]
        [ResourcePublishAttribute(enResourceType.SimMeshSource)]
        public static string DefaultSimMesh
        {
            get;
            set;
        } = "resources/default/editor/mesh/box.vms";

        [DataValueAttribute("EditorResourcePath")]
        [ResourcePublishAttribute(enResourceType.Folder)]
        public static string EditorResourcePath
        {
            get;
            set;
        } = "editorresources";

        static Guid mServerAltitudeAssistMeshTemplate = CSUtility.Support.IHelper.GuidTryParse("99661b7c-f99d-4813-8ef1-961cfa2e49d4");
        [DataValueAttribute("ServerAltitudeAssistMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid ServerAltitudeAssistMeshTemplate
        {
            get { return mServerAltitudeAssistMeshTemplate; }
            set { mServerAltitudeAssistMeshTemplate = value; }
        }
        static Guid mLineCheckAssistMeshTemplate = CSUtility.Support.IHelper.GuidTryParse("0cb73630-cf69-4eb3-878e-eeaa27d7722c");
        [DataValueAttribute("LineCheckAssistMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid LineCheckAssistMeshTemplate
        {
            get { return mLineCheckAssistMeshTemplate; }
            set { mLineCheckAssistMeshTemplate = value; }
        }
        
        [DataValueAttribute("AudioOutRangeMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid AudioOutRangeMeshTemplate
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("83dd089a-50fd-4527-81c7-310ff782dfb8");
        [DataValueAttribute("AudioInRangeMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid AudioInRangeMeshTemplate
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("6ae6f75a-6db0-460f-bf8c-4d8312a79359");
        [DataValueAttribute("AudioObjectMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid AudioObjectMeshTemplate
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("c5c04353-1282-4de7-97aa-daa36cc7123a");
        
        [DataValueAttribute("DynamicBlockMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid DynamicBlockMeshTemplate
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("fae048af-c9a1-4069-8234-2a4f5c7f862a");

        // 方向光图标模型
        [DataValueAttribute("DirLightMeshTemplate")]
        [ResourcePublishAttribute(enResourceType.MeshTemplate)]
        public static Guid DirLightMeshTemplate
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("6f457d20-a8de-4523-aa79-3af71aeed8c7");
        // 方向光箭头材质
        [DataValueAttribute("DirLightArrowTech")]
        [ResourcePublishAttribute(enResourceType.Technique)]
        public static Guid DirLightArrowTech
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("fe9bed25-039e-4fae-8383-720e6c4a8edb");
        // 箭头模型
        [DataValueAttribute("ArrowMesh")]
        [ResourcePublishAttribute(enResourceType.MeshSource)]
        public static string ArrowMesh
        {
            get;
            set;
        } = "editorresources/mesh/arrow.vms";

        #region Editor

        [DataValueAttribute("EditorBackgroundSkyMesh")]
        // 编辑器3D窗口背景
        public static Guid EditorBackgroundSkyMesh
        {
            get;
            set;
        } = CSUtility.Support.IHelper.GuidTryParse("00038fa4-077a-4f5d-aa45-e4104a4196a6");

        #endregion

        IFileConfig()
		{
		}
    }
}
