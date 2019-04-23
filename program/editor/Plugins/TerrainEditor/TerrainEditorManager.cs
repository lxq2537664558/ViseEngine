using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCore.MsgProc;
using System.Reflection;

namespace TerrainEditor
{
    public class TerrainUndoRedoData
    {
        public bool mIncrease;
        public bool mCanSetActorHeight;
        public TerrainPanel.enTerrainToolType mToolType;
        public Dictionary<uint, Dictionary<uint, int>> mModifyDataDic;
        public Dictionary<uint, Dictionary<uint, int>> mOldModifyDataDic;

        public TerrainUndoRedoData(bool increase, bool canSetActorHeight, TerrainPanel.enTerrainToolType toolType, Dictionary<uint, Dictionary<uint, int>> oldModifyDataDic, Dictionary<uint, Dictionary<uint, int>> modifyDataDic)
        {
            mIncrease = increase;
            mCanSetActorHeight = canSetActorHeight;
            mToolType = toolType;
            mOldModifyDataDic = oldModifyDataDic;
            mModifyDataDic = modifyDataDic;
        }
    }

    public class TerrainEditorManager : MsgReceiver
    {
//         static TerrainEditorManager smInstance = new TerrainEditorManager();
//         public static TerrainEditorManager Instance
//         {
//             get { return smInstance; }
//         }

        int mCurIndex = 0;
        TerrainPanel mTerrainPanel = null;
        List<TerrainUndoRedoData> mTerrainUndoRedoDatas = new List<TerrainUndoRedoData>();

        public TerrainUndoRedoData GetTerrainUndoRedoDataWithIndex()
        {
            if (mCurIndex >= mTerrainUndoRedoDatas.Count)
                return null;

            return mTerrainUndoRedoDatas[mCurIndex];
        }

        public void RedoTerrainModify()
        {
            mCurIndex++;

            if (mCurIndex >= mTerrainUndoRedoDatas.Count)
                mCurIndex = mTerrainUndoRedoDatas.Count - 1;
        }

        public void UndoTerrainModify()
        {
            if (mCurIndex >= mTerrainUndoRedoDatas.Count)
                mCurIndex = mTerrainUndoRedoDatas.Count - 1;

            mCurIndex--;
        }

        public void AddTerrainModifyData(TerrainUndoRedoData data)
        {
            ClearDirtyTerrainData();
            mTerrainUndoRedoDatas.Add(data);
            mCurIndex = mTerrainUndoRedoDatas.Count - 1;
        }

        public void AddTerrainModifyData(Dictionary<uint, Dictionary<uint, int>> oldTerrainModifyData, Dictionary<uint, Dictionary<uint, int>> terrainModifyData, TerrainPanel.enTerrainToolType type, bool Increase, bool canSetActorHeight)
        {
            var data = new TerrainUndoRedoData(Increase, canSetActorHeight, type, oldTerrainModifyData, terrainModifyData);

            AddTerrainModifyData(data);
        }

        public void ClearAllTerrainData()
        {
            mTerrainUndoRedoDatas.Clear();
        }

        void ClearDirtyTerrainData()
        {
            if (mCurIndex >= mTerrainUndoRedoDatas.Count)
                return;

            while (true)
            {
                if (mCurIndex == mTerrainUndoRedoDatas.Count - 1)
                    break;

                mTerrainUndoRedoDatas.Remove(mTerrainUndoRedoDatas[mTerrainUndoRedoDatas.Count - 1]);
            }

        }

        private FBehaviorProcess mLBDownBehavior;
        private FBehaviorProcess mLBUpBehavior;
        public TerrainEditorManager(TerrainPanel terrainPanel)
        {
            mTerrainPanel = terrainPanel;

            CCore.Engine.Instance.Client.MsgRecieverMgr.RegReciever(this);

            mLBUpBehavior = OnLBUp;
            mLBDownBehavior = OnLBDown;
        }

//         public void SetTerrainPanel(TerrainPanel terrainPanel)
//         {
//             mTerrainPanel = terrainPanel;
//         }
        
        int OnLBDown(BehaviorParameter param)
        {
            if (mTerrainPanel == null)
                return 0;
            if (EditorCommon.WorldEditMode.Instance.EditMode == EditorCommon.WorldEditMode.enEditMode.Edit_Terrain)
                mTerrainPanel.OnLBDown(param);
            return 0;
        }

        int OnLBUp(BehaviorParameter param)
        {
            if (mTerrainPanel == null)
                return 0;
            if (EditorCommon.WorldEditMode.Instance.EditMode == EditorCommon.WorldEditMode.enEditMode.Edit_Terrain)
                mTerrainPanel.OnLBUp(param);
            return 0;
        }

        public override FBehaviorProcess FindBehavior(BehaviorParameter bhInit)
        {
            switch (bhInit.GetBehaviorType())
            {
                case (int)BehaviorType.BHT_LB_Down:
                    return mLBDownBehavior;
                case (int)BehaviorType.BHT_LB_Up:
                    return mLBUpBehavior;
            }

            return base.FindBehavior(bhInit);
        }


    }
}
