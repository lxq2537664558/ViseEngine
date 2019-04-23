using System;
using System.Collections.Generic;

namespace CCore.Material
{
    /// <summary>
    /// 材质模板类
    /// </summary>
    public class MaterialTemplate
    {
        IntPtr mMaterial;   // model3::vStandMaterial
        /// <summary>
        /// 材质模板的指针
        /// </summary>
        public IntPtr Material
        {
            get { return mMaterial; }
        }
        /// <summary>
        /// 只读属性，材质模板文件的名称
        /// </summary>
        public string FileName
        {
            get
            {
                var str = DllImportAPI.v3dStagedMaterialBase_GetMaterialTag(mMaterial);
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str);
            }
        }
        /// <summary>
        /// 只读属性，材质模板的主要数据
        /// </summary>
        public string Main
        {
            get
            {
                var str = DllImportAPI.v3dStagedMaterialBase_GetMain(mMaterial);
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(str);
            }
        }
        /// <summary>
        /// 材质模板的文件路径
        /// </summary>
        public string Path
        {
            get { return ""; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mtl">材质模板指针</param>
        public MaterialTemplate(IntPtr mtl)
        {
            mMaterial = mtl;
            unsafe
            {
                DllImportAPI.v3dStagedMaterialBase_AddRef(mMaterial);
            }
        }
        /// <summary>
        /// 析构函数，释放内存
        /// </summary>
        ~MaterialTemplate()
        {
            unsafe
            {
                DllImportAPI.v3dStagedMaterialBase_Release(mMaterial);
                mMaterial = IntPtr.Zero;
            }
        }
        /// <summary>
        /// 获取所有的Tech名称
        /// </summary>
        /// <returns>返回Tech名称的列表</returns>
        public List<string> GetAllTechNames()
        {
            unsafe
            {
                var result = new List<string>();
                int strCount = 0;
                void** techNames = DllImportAPI.vStandMaterial_GetTechNames(mMaterial, &strCount);
                for (int i = 0; i < strCount; i++)
                {
                    result.Add(System.Runtime.InteropServices.Marshal.PtrToStringAnsi((IntPtr)(techNames[i])));
                }
                DllImportAPI.v3dStagedMaterialBase_DeleteStrings(techNames, strCount);
                return result;
            }
        }
    }
    /// <summary>
    /// 材质模板的参数类
    /// </summary>
	[System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class MaterialParameter
    {
        string mMaterial;
        /// <summary>
        /// 当前材质的名称
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Material")]
        public string Material
        {
            get { return mMaterial; }
            set { mMaterial = value; }
        }
        string mTech;
        /// <summary>
        /// 材质的Tech
        /// </summary>
        [CSUtility.Support.DataValueAttribute("Tech")]
        public string Tech
        {
            get { return mTech; }
            set { mTech = value; }
        }
        /// <summary>
        /// 材质的构造函数
        /// </summary>
        public MaterialParameter()
        {
            mMaterial = "";
            mTech = "Tech0";
        }
        /// <summary>
        /// 材质模板的带参构造函数
        /// </summary>
        /// <param name="m">材质的名称</param>
        /// <param name="t">材质的Tech</param>
        public MaterialParameter(string m, string t)
        {
            mMaterial = m;
            mTech = t;
        }
    }
    /// <summary>
    /// 材质类
    /// </summary>
    [System.ComponentModel.TypeConverterAttribute( "System.ComponentModel.ExpandableObjectConverter" )]
    public class Material
    {
        /// <summary>
        /// 材质实例的指针
        /// </summary>
        protected IntPtr mMaterialPtr; // model3::v3dStagedMaterialInstance
        /// <summary>
        /// 只读属性，材质实例的指针
        /// </summary>
        public IntPtr MaterialPtr
        {
            get { return mMaterialPtr; }
        }
        /// <summary>
        /// 材质的构造函数，创建对象实例
        /// </summary>
        public Material()
        {
            unsafe
            {
                mMaterialPtr = DllImportAPI.v3dStagedMaterialInstance_New();
            }
        }
        /// <summary>
        /// 带参的构造函数
        /// </summary>
        /// <param name="mtl">材质对象的指针</param>
        public Material(IntPtr mtl)
        {
            unsafe
            {
                DllImportAPI.v3dStagedMaterialInstance_AddRef(mtl);
                mMaterialPtr = mtl;
            }
        }
        /// <summary>
        /// 材质的带参构造函数
        /// </summary>
        /// <param name="tpl">材质模板对象</param>
        /// <param name="techName">材质模板的Tech名称</param>
        public Material(MaterialTemplate tpl, string techName)
        {
            unsafe
            {
                mMaterialPtr = DllImportAPI.v3dStagedMaterialInstance_New();
                var t = DllImportAPI.vStandMaterial_GetTechnique(tpl.Material, techName);
                _Initialize(tpl.Material, t);
            }
        }
        /// <summary>
        /// 析构函数，删除对象，释放内存
        /// </summary>
        ~Material()
        {
            Cleanup();
        }
        /// <summary>
        /// 删除对象并将指针置空
        /// </summary>
        public void Cleanup()
        {
            unsafe
            {
                if (mMaterialPtr != IntPtr.Zero)
                {
                    DllImportAPI.v3dStagedMaterialInstance_Release(mMaterialPtr);
                    mMaterialPtr = IntPtr.Zero;
                }
            }
        }
        /// <summary>
        /// 使用字符为该对象分配唯一的ID
        /// </summary>
        /// <param name="key">字符关键字</param>
        /// <returns>分配的唯一的ID</returns>
        public static UInt64 AssignUniqueIDWithString(string key)
        {
            return DllImportAPI.v3dStagedMaterialBase_AssignUniqueIDWithString(key);
        }
        /// <summary>
        /// 加载高清图片纹理
        /// </summary>
        public void LoadRefTexture()
        {
            DllImportAPI.v3dStagedMaterialInstance_LoadRefTexture(mMaterialPtr);
        }
        /// <summary>
        /// 设置材质的名称及其属性
        /// </summary>
        /// <param name="name">材质的名称</param>
        /// <param name="v">材质的单个属性</param>
        /// <returns>返回该材质的指针</returns>
        public IntPtr SetFloat(string name,float v)
        {
            unsafe
            {
                name = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + name;
                return DllImportAPI.v3dStagedMaterialInstance_SetFloat(mMaterialPtr, name, v);
            }
        }
        /// <summary>
        /// 设置材质的名称及其二维变量属性
        /// </summary>
        /// <param name="name">材质的名称</param>
        /// <param name="v">材质的二维变量属性</param>
        /// <returns>返回该材质的指针</returns>
        public IntPtr SetFloat2(string name, ref SlimDX.Vector2 v)
        {
            unsafe
            {
                fixed (SlimDX.Vector2* pinV = &v)
                {
                    name = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + name;
                    return DllImportAPI.v3dStagedMaterialInstance_SetFloat2(mMaterialPtr, name, pinV);
                }
            }
        }
        /// <summary>
        /// 设置材质的名称及其三维变量属性
        /// </summary>
        /// <param name="name">材质的名称</param>
        /// <param name="v">材质的三维变量属性</param>
        /// <returns>返回该材质的指针</returns>
        public IntPtr SetFloat3(string name, ref SlimDX.Vector3 v)
        {
            unsafe
            {
                name = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + name;
                fixed (SlimDX.Vector3* pinV = &v)
                    return DllImportAPI.v3dStagedMaterialInstance_SetFloat3(mMaterialPtr, name, pinV);
            }
        }
        /// <summary>
        /// 设置材质的名称及其四维属性
        /// </summary>
        /// <param name="name">材质的名称</param>
        /// <param name="v">材质的四维属性，比如其旋转四元数</param>
        /// <returns>返回该材质的指针</returns>
        public IntPtr SetFloat4(string name, ref SlimDX.Vector4 v)
        {
            unsafe
            {
                name = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + name;
                fixed (SlimDX.Vector4* pinV = &v)
                    return DllImportAPI.v3dStagedMaterialInstance_SetFloat4(mMaterialPtr, name, pinV);
            }
        }
        /// <summary>
        /// 设置材质的名称及其矩阵属性
        /// </summary>
        /// <param name="name">材质的名称</param>
        /// <param name="v">材质的矩阵属性，比如其位置矩阵</param>
        /// <returns>返回该材质的指针</returns>
        public IntPtr SetFloat4x4(string name, ref SlimDX.Matrix v)
        {
            unsafe
            {
                name = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + name;
                fixed (SlimDX.Matrix* pinV = &v)
                    return DllImportAPI.v3dStagedMaterialInstance_SetFloat4x4(mMaterialPtr, name, pinV);
            }
        }
        /// <summary>
        /// 设置材质的纹理
        /// </summary>
        /// <param name="name">材质名称</param>
        /// <param name="v">纹理名称</param>
        /// <returns>返回材质指针</returns>
        public IntPtr SetTexture(string name, CCore.Graphics.Texture v)
        {
            unsafe
            {
                name = CCore.Material.MaterialShaderVarInfo.ValueNamePreString + name;
                return DllImportAPI.v3dStagedMaterialInstance_SetTexture(mMaterialPtr, name, v.TexturePtr);
            }
        }
        /// <summary>
        /// 通过shader变量设置变量
        /// </summary>
        /// <param name="var">shader变量</param>
        /// <param name="v">变量</param>
        public void SetFloatByShaderVar(IntPtr var, float v)
        {
            unsafe
            {
                DllImportAPI.v3dStagedMaterialInstance_SetFloatByShaderVar(var, v);
            }
        }
        /// <summary>
        /// 通过shader变量设置二维数组
        /// </summary>
        /// <param name="var">shader变量</param>
        /// <param name="v">二维数组</param>
        public void SetFloat2ByShaderVar(IntPtr var, ref SlimDX.Vector2 v)
        {
            unsafe
            {
                fixed (SlimDX.Vector2* pinV = &v)
                {
                    DllImportAPI.v3dStagedMaterialInstance_SetFloat2ByShaderVar(var, pinV);
                }
            }
        }
        /// <summary>
        /// 通过shader变量设置三维数组
        /// </summary>
        /// <param name="var">shader变量</param>
        /// <param name="v">三维数组</param>
        public void SetFloat3ByShaderVar(IntPtr var, ref SlimDX.Vector3 v)
        {
            unsafe
            {
                fixed (SlimDX.Vector3* pinV = &v)
                    DllImportAPI.v3dStagedMaterialInstance_SetFloat3ByShaderVar(var, pinV);
            }
        }
        /// <summary>
        /// 通过shader变量设置四元数
        /// </summary>
        /// <param name="var">shader变量</param>
        /// <param name="v">四元数</param>
        public void SetFloat4ByShaderVar(IntPtr var, ref SlimDX.Vector4 v)
        {
            unsafe
            {
                fixed (SlimDX.Vector4* pinV = &v)
                    DllImportAPI.v3dStagedMaterialInstance_SetFloat4ByShaderVar(var, pinV);
            }
        }
        /// <summary>
        /// 通过shader变量设置矩阵
        /// </summary>
        /// <param name="var">shader变量</param>
        /// <param name="v">矩阵</param>
        public void SetFloat4x4ByShaderVar(IntPtr var, ref SlimDX.Matrix v)
        {
            unsafe
            {
                fixed (SlimDX.Matrix* pinV = &v)
                    DllImportAPI.v3dStagedMaterialInstance_SetFloat4x4ByShaderVar(var, pinV);
            }
        }
        /// <summary>
        /// 通过shader变量设置纹理
        /// </summary>
        /// <param name="var">shader变量</param>
        /// <param name="v">纹理对象</param>
        public void SetTextureByShaderVar(IntPtr var, CCore.Graphics.Texture v)
        {
            unsafe
            {
                DllImportAPI.v3dStagedMaterialInstance_SetTextureByShaderVar(var, v.TexturePtr);
            }
        }
        /// <summary>
        /// 材质是否有效
        /// </summary>
        /// <returns>返回该对象是否生效</returns>
		public bool IsMaterialValid()
		{
			return mMaterialPtr != IntPtr.Zero;
		}

        //public IntPtr GetMaterialPtr()
        //{
        //    return (IntPtr)(mMaterial);
        //}
        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="bForce">是否强制从磁盘加载</param>
        /// <param name="time">加载时间</param>
		public void PreUse( bool bForce , Int64 time )
        {
            unsafe
            {
                DllImportAPI.v3dStagedMaterialInstance_PreUse(mMaterialPtr, bForce, time);
            }
        }
        /// <summary>
        /// 材质的初始化
        /// </summary>
        /// <param name="m">材质的指针</param>
        /// <param name="t">纹理的指针</param>
        /// <returns>初始化成功返回true，否则返回false</returns>
        public bool _Initialize( IntPtr m , IntPtr t )
        {
            unsafe
            {
                DllImportAPI.v3dStagedMaterialInstance_SetMaterial(mMaterialPtr, m, t);
                return true;
            }
        }
    }
}
