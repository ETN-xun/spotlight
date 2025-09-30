namespace View.Base
{
    public interface IBaseView
    {
        public int ViewId { get; set; }
        public bool IsInit { get; set; }
        public void Init();
        public void Open(params object[] args);
        public void Close(params object[] args);
        public void Destroy();
        
        public void SetVisible(bool visible);
        
    }
}