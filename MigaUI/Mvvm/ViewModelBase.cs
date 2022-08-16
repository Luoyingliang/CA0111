namespace Acorisoft.Miga.UI.Mvvm
{
    public abstract class ViewModelBase : PropertyChanger
    {
        private string _title;

        protected ViewModelBase()
        {
            Initializing();
        }

        private void Initializing()
        {
            OnInitializing();
        }

        protected internal virtual void OnStart(){}
        protected internal virtual void OnStop(){}

        protected internal virtual void OnParameterReceived(object parameter)
        {
            
        }

        /// <summary>
        /// 设置 <see cref="Title"/> 属性。
        /// </summary>
        /// <param name="title">新的标题</param>
        protected void SetTitle(string title) => SetValue(ref _title, title);

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnInitializing()
        {
        }
        
        /// <summary>
        /// 是否为临时节点
        /// </summary>
        public bool IsTemporaryEntry { get; protected internal set; }
        
        /// <summary>
        /// 是否跳过释放缓解
        /// </summary>
        public bool SkipDisposePass { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="Title"/> 属性。
        /// </summary>
        public string Title
        {
            get => _title;
            init => SetValue(ref _title, value);
        }
    }
}