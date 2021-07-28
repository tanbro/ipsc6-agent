using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ipsc6.agent.wpfapp
{

    public class ObservableContent<T> : ObservableObject where T : class
    {
        private T _content;

        public T Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public ObservableContent() { }

        public ObservableContent(T content)
        {
            Content = content;
        }
    }

}
