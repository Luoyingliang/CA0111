using System.Threading.Tasks;

namespace Acorisoft.Miga.UI.Mvvm
{
    /// <summary>
    /// 默认的对话框
    /// </summary>
    public abstract class DialogAware : ViewModelBase
    {
        internal readonly TaskCompletionSource<object> _signal;
        
        protected DialogAware()
        {
            IsOperationFinished = false;
            _signal = new TaskCompletionSource<object>();
        }

        private protected virtual void Construct()
        {
            ConstructOverride();
        }

        /// <summary>
        /// 构建对话框返回结果。
        /// </summary>
        protected abstract void ConstructOverride();
        
        /// <summary>
        /// 是否可以创建对象。
        /// </summary>
        /// <returns>返回一个值，如果可以创建对象，则返回true，否则返回false。</returns>
        protected abstract bool CanCompleted();

        /// <summary>
        /// 结束对话框，并返回操作成功。
        /// </summary>
        public void Completed()
        {
            // avoid set result twice!
            if (IsOperationFinished)
            {
                return;
            }

            Construct();
            IsOperationFinished = true;
            IsCompleted = true;
            _signal.SetResult(Result);
            OnStop();
        }

        /// <summary>
        /// 结束对话框，并返回操作失败。
        /// </summary>
        public void Cancel()
        {       
            // avoid set result twice!
            if (IsOperationFinished)
            {
                return;
            }

            Construct();
            IsOperationFinished = true;
            IsCompleted = false;
            _signal.SetResult(Result);
            OnStop();
        }

        /// <summary>
        /// 是否可以完成
        /// </summary>
        internal bool CanFinish => CanCompleted();
        
        /// <summary>
        /// 是否已经操作完成了
        /// </summary>
        internal bool IsOperationFinished { get; private set; }

        /// <summary>
        /// 对话框结果
        /// </summary>
        public object Result { get; protected set; }
        
        
        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool IsCompleted { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PageAware OwnerPage { get; internal set; }
    }

    /// <summary>
    /// 可编辑的对象
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityAware<TEntity> : DialogAware
    {
        private readonly TEntity _entity;

        private protected sealed override void Construct()
        {
            ConstructOverride();
            
            if (IsEditMode)
            {
                DeconstructOverride(_entity);
            }
        }

        /// <summary>
        /// 解构实体。
        /// </summary>
        protected abstract void DeconstructOverride(TEntity entity);
        
        /// <summary>
        /// 是否为编辑模式。
        /// </summary>
        public bool IsEditMode { get; init; }

        /// <summary>
        /// 获取当前编辑的对象。
        /// </summary>
        public TEntity Entity
        {
            get => _entity;
            init
            {
                IsEditMode = value is not null;
                _entity = value;
                DeconstructOverride(value);
            }
        }
    }
}