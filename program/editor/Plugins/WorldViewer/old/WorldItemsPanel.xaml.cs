using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WorldViewer
{
    public class WorldItem : INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public delegate void Delegate_OnVisibleChanged(bool visible);
        public event Delegate_OnVisibleChanged OnVisibleChanged;

        ObservableCollection<WorldItem> mWorldItems = new ObservableCollection<WorldItem>();
        public ObservableCollection<WorldItem> WorldItems
        {
            get { return mWorldItems; }
            set { mWorldItems = value; }
        }

        public int GetChildrenCount()
        {
            int retCount = 0;
            foreach(var item in WorldItems)
            {
                retCount += item.GetChildrenCount();
            }

            retCount += WorldItems.Count;
            return retCount;
        }

        CCore.World.Actor mHostActor = null;
        public CCore.World.Actor HostActor
        {
            get { return mHostActor; }
            set
            {
                mHostActor = value;
                if (mHostActor != null)
                {
                    DisplayName = GetDisplayName(mHostActor);
                    ActorVisible = mHostActor.Visible;

                    switch (mHostActor.GameType)
                    {
#warning 提出通用方法，以便对象扩展
                        case (UInt16)CSUtility.Component.EActorGameType.Effect:
                            EffectResetVisibility = Visibility.Visible;
                            break;
                    }

                    if (mHostActor.Visual is CCore.Mesh.Mesh)
                    {
                        var mesh = mHostActor.Visual as CCore.Mesh.Mesh;
                        if (mesh != null)
                        {
                            var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                            if (meshInit != null)
                            {
                                if (meshInit.MeshTemplate == null)
                                {
                                    ErrorVisibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                }
            }
        }

        Visibility mErrorVisibility = Visibility.Collapsed;
        public Visibility ErrorVisibility
        {
            get { return mErrorVisibility; }
            set
            {
                mErrorVisibility = value;
                OnPropertyChanged("ErrorVisibility");
            }
        }

        string mDisplayName = "";
        public string DisplayName
        {
            get { return mDisplayName; }
            set
            {
                mDisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        Visibility mEffectResetVisibility = Visibility.Collapsed;
        public Visibility EffectResetVisibility
        {
            get { return mEffectResetVisibility; }
            set
            {
                mEffectResetVisibility = value;
                OnPropertyChanged("EffectResetVisibility");
            }
        }

        bool mActorVisible = true;
        public bool ActorVisible
        {
            get { return mActorVisible; }
            set
            {
                mActorVisible = value;

                if (HostActor != null && HostActor.Visible != mActorVisible)
                    HostActor.Visible = mActorVisible;

                if (OnVisibleChanged != null)
                    OnVisibleChanged(mActorVisible);

                OnPropertyChanged("ActorVisible");
            }
        }

        Visibility mItemVisibility = Visibility.Visible;
        public Visibility ItemVisibility
        {
            get { return mItemVisibility; }
            set
            {
                mItemVisibility = value;

                OnPropertyChanged("ItemVisibility");
            }
        }

        UInt32 mNeighborSide = 1;
        public UInt32 NeighborSide
        {
            get { return mNeighborSide; }
            set
            {
                mNeighborSide = value;

                CCore.Client.MainWorldInstance.SetNeighborSide(mNeighborSide);

                OnPropertyChanged("NeighborSide");
            }
        }

        public void OnLayerVisibleChanged(bool visible)
        {
            ActorVisible = visible;
        }

        public static string GetDisplayName(CCore.World.Actor actor)
        {
            if (actor == null)
                return "";

#warning 通用对象处理
            /*switch (actor.GameType)
            {
                case CSUtility.Component.EActorGameType.Player:
                    {
                        return "Player:" + ((FrameSet.Role.RoleActor)actor).RoleName;
                    }

                case CSUtility.Component.EActorGameType.Npc:
                    {
                        return "NPC:" + ((FrameSet.Role.RoleActor)actor).RoleName;
                    }

                case CSUtility.Component.EActorGameType.NpcInitializer:
                    {
                        var retName = "NPCInitializer:";
                        var npcInit = actor.ActorInit as CSUtility.Data.NPCInitializerActorInit;
                        if (string.IsNullOrEmpty(npcInit.NPCData.Name) && npcInit.NPCData.Template != null)
                        {
                            retName += npcInit.NPCData.Template.NickName;
                        }
                        else
                            retName += npcInit.NPCData.Name;

                        return retName;
                    }

                case CSUtility.Component.EActorGameType.Common:
                    {
                        if (actor.Visual is CCore.Mesh.Mesh)
                        {
                            var mesh = actor.Visual as CCore.Mesh.Mesh;
                            if (mesh == null)
                                return actor.GetType().Name + ":" + actor.ActorName;

                            var meshInit = mesh.VisualInit as CCore.Mesh.MeshInit;
                            if (meshInit == null || meshInit.MeshTemplate == null)
                                return actor.GetType().Name + ":" + actor.ActorName;

                            return actor.GetType().Name + ":" + meshInit.MeshTemplate.NickName;
                        }
                        else
                            return actor.GetType().Name + ":" + actor.ActorName;
                    }

                case CSUtility.Component.EActorGameType.Trigger:
                    {
                        var triggerInit = actor.ActorInit as CSUtility.Data.MapObject.Trigger.TriggerActorInit;
                        return actor.GetType().Name + ":" + triggerInit.TriggerData.TriggerName;
                    }

                default:
                    return actor.GetType().Name + ":" + actor.ActorName;
            }*/
            return "";
        }
    }

    /// <summary>
    /// WorldItemsPanel.xaml 的交互逻辑
    /// </summary>
    public partial class WorldItemsPanel : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        ObservableCollection<WorldItem> mWorldItems = new ObservableCollection<WorldItem>();
        public ObservableCollection<WorldItem> WorldItems
        {
            get { return mWorldItems; }
            set { mWorldItems = value; }
        }

        string mInfoString = "";
        public string InfoString
        {
            get { return mInfoString; }
            set { mInfoString = value; }
        }

        public WorldItemsPanel()
        {
            InitializeComponent();
        }

        public void UpdateCountString()
        {
            int count = 0;
            foreach(var item in mWorldItems)
            {
                count += item.GetChildrenCount();
            }
            count += mWorldItems.Count;

            InfoString = count + "个对象";
        }

        public void UpdateActors()
        {
            WorldItems.Clear();

            var prefabs = CCore.Client.MainWorldInstance.GetActors((UInt16)(CSUtility.Component.EActorGameType.Prefab));
            var allActors = CCore.Client.MainWorldInstance.GetActors((UInt16)(CSUtility.Component.EActorGameType.Unknow));
            foreach(var actor in allActors)
            {
                if (actor == null)
                    continue;

                bool bContainInPrefab = false;
                foreach(CCore.World.Prefab.Prefab prefab in prefabs)
                {
                    if(prefab.ContainActor(actor.Id))
                    {
                        bContainInPrefab = true;
                        break;
                    }
                }
                if (bContainInPrefab)
                    continue;

                var item = new WorldItem();
                item.HostActor = actor;
                WorldItems.Add(item);

            }

            UpdateCountString();
        }
    }
}
