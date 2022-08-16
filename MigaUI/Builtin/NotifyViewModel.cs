namespace Acorisoft.Miga.UI.Builtin
{
    public class NotifyViewModel : DialogAware
    {
        protected override void ConstructOverride() {}

        protected override bool CanCompleted() => true;
        
        private string _content;

        /// <summary>
        /// 获取或设置 <see cref="Content"/> 属性。
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetValue(ref _content, value);
        }
    }
}