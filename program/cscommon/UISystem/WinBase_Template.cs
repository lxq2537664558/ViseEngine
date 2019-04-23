using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UISystem
{
    public partial class WinBase
    {
        protected bool mIsInTemplate = false;
        [Browsable(false)]
        public bool IsTemplateControl
        {
            get { return mIsInTemplate; }
            set
            {
                mIsInTemplate = value;

                if (mIsInTemplate)
                {
                    VisibleInTreeView = Visibility.Collapsed;
                }
                else
                    VisibleInTreeView = Visibility.Visible;

                foreach (var child in ChildWindows)
                {
                    child.IsTemplateControl = mIsInTemplate;
                }

                OnPropertyChanged("IsTemplateControl");
            }
        }

        protected Visibility mVisibleInTreeView = Visibility.Visible;
        [Browsable(false)]
        public Visibility VisibleInTreeView
        {
            get { return mVisibleInTreeView; }
            set
            {
                mVisibleInTreeView = value;

                OnPropertyChanged("VisibleInTreeView");
            }
        }

        protected Guid mCopyedFromId = Guid.Empty;
        [Browsable(false)]
        public Guid CopyedFromId
        {
            get { return mCopyedFromId; }
        }

        protected Guid mTemplateId = Guid.Empty;
        [Browsable(false)]
        public Guid TemplateId
        {
            get { return mTemplateId; }
            set
            {
                if (mTemplateId == value)
                    return;

                mTemplateId = value;

                var templateInfo = Template.TemplateMananger.Instance.FindControlTemplate(mTemplateId);
                if (templateInfo != null)
                {
                    //foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(templateInfo.ControlTemplate.TargetControl))
                    //{
                    //    if (property.Name == "TemplateId" ||
                    //        property.Name == "Parent")
                    //        continue;

                    //    property.SetValue(this, property.GetValue(templateInfo.ControlTemplate.TargetControl));
                    //}

                    this.CopyFrom(templateInfo.ControlTemplate.TargetControl,
                                            new List<string> { "TemplateId",
                                                               "Parent",
                                                               "IgnoreSaver",
                                                               "VisibleInTreeView"});
                    foreach (var ctrl in templateInfo.ControlTemplate.ChildWindows)
                    {
                        if (ctrl == templateInfo.ControlTemplate.TargetControl)
                            continue;

                        var copyedCtrl = ctrl.GetType().Assembly.CreateInstance(ctrl.GetType().ToString()) as UISystem.WinBase;
                        copyedCtrl.CopyFrom(ctrl, new List<string> { "Parent", "ChildWindows" }, false, true);
                        copyedCtrl.IsTemplateControl = true;
                        copyedCtrl.IgnoreSaver = true;
                        copyedCtrl.VisibleInTreeView = Visibility.Collapsed;
                        copyedCtrl.Parent = this;
                    }
                    //this.CopyFrom(templateInfo);

                    
                    // Command处理
                    BuildCommandBinding(this, true, false);

                    // 记录从模板创建的命令
                    foreach (var commandInfo in this.CommandBindingInfoDictionary)
                    {
                        foreach (var info in commandInfo.Value)
                        {
                            CommandFromTemplateList.Add(commandInfo.Key + "_" + info);
                        }
                    }

                    // Trigger处理
                    if (templateInfo.ControlTemplate.UITriggerManager.IsHaveTrigger)
                    {
                        if (mTemplateTriggerManager == null)
                            mTemplateTriggerManager = new Trigger.UITriggerManager(this);
                        
                        foreach (var trigger in templateInfo.ControlTemplate.UITriggerManager.PropertyTriggerList)
                        {
                            Trigger.PropertyTrigger propertyTrigger = new Trigger.PropertyTrigger(mTemplateTriggerManager);

                            foreach (var condition in trigger.Conditions)
                            {
                                var targetControl = this.FindControl(condition.TargetId);
                                Trigger.PropertyTriggerConditon copyedCondition = new Trigger.PropertyTriggerConditon(targetControl, condition.TargetProperty, condition.ValueOperator, condition.TargetValue);
                                propertyTrigger.AddCondition(copyedCondition);
                            }

                            foreach (var action in trigger.Actions)
                            {
                                if (action is Trigger.TriggerAction_Property)
                                {
                                    var proAction = action as Trigger.TriggerAction_Property;
                                    var targetControl = this.FindControl(proAction.TargetId);
                                    Trigger.TriggerAction_Property copyedAction = new Trigger.TriggerAction_Property(targetControl, proAction.TargetProperty, proAction.TargetValue);
                                    propertyTrigger.AddAction(copyedAction);
                                }
                            }

                            mTemplateTriggerManager.AddPropertyTrigger(propertyTrigger);
                        }
                    }
                    templateInfo.ControlTemplate.UITriggerManager.BuildTriggers(this);

                    // 绑定对象处理
                    if (templateInfo.ControlTemplate.PropertyBindManager.IsHaveBind)
                    {
                        templateInfo.ControlTemplate.PropertyBindManager.BuildBindings(this);
                    }
                    

                    OnSetTemplateId(templateInfo);
                }

                OnPropertyChanged("TemplateId");
            }
        }

        protected virtual void OnSetTemplateId(Template.ControlTemplateInfo templateInfo)
        {
        }

        //public bool CopyFrom(Template.ControlTemplateInfo templateInfo)
        //{
        //    this.CopyFrom(templateInfo.ControlTemplate.TargetControl,
        //                            new List<string> { "TemplateId",
        //                                               "Parent",
        //                                               "IgnoreSaver",
        //                                               "VisibleInTreeView"});

        //    //foreach (var ctrl in templateInfo.ControlTemplate.ChildWindows)
        //    //{
        //    //    if (ctrl == templateInfo.ControlTemplate.TargetControl)
        //    //        continue;

        //    //    var copyedCtrl = ctrl.GetType().Assembly.CreateInstance(ctrl.GetType().ToString()) as UISystem.WinBase;
        //    //    copyedCtrl.CopyFrom(ctrl, new List<string> { "Parent", "ChildWindows" }, false, true);
        //    //    copyedCtrl.IsTemplateControl = true;
        //    //    copyedCtrl.IgnoreSaver = true;
        //    //    copyedCtrl.VisibleInTreeView = Visibility.Collapsed;
        //    //    copyedCtrl.Parent = this;
        //    //}

        //    return true;
        //}

        //public bool CopyFromTemplate(WinBase templateControl)
        //{

        //}

        // 模板Trigger处理
        private Trigger.UITriggerManager mTemplateTriggerManager = null;
        // 模板控件绑定处理
        //private Bind.PropertyBindManager mPropertyBindManager = null;
    }
}
