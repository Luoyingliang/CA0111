namespace Acorisoft.Miga.UI
{
    public class MGSubView  : MGViewHostBase
    {
        /// <summary>
        /// 重写该方法来决定是否导航
        /// </summary>
        /// <param name="vm">将要导航到的视图模型。</param>
        protected override void OnViewModelChanged(ViewModelBase vm)
        {
            //
            // 获得页面
            var page = MGApp.Locate(vm);

            //
            //
            if (page is null)
            {
                return;
            }
            
            //
            // 设置数据上下文
            page.DataContext = vm;

            //
            // 设置内容
            Content = page;
        }

        public override string ToString()
        {
            return Content?.GetType().Name ?? "无内容";
        }
    }
}