namespace AspNetBlog.ViewModels
{
    public class ResultViewModel<T>
    {
        public ResultViewModel(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }

        public ResultViewModel(T data)
        {
            Data = data;
        }

        public ResultViewModel(List<string> errors)
        {
            Errors = errors;
        }

        public ResultViewModel(string error)
        {
            Errors.Add(error);
        }

        public T Data { get; private set; }

        //O new() substitui a inicialização do construtor. e o new
        //ja sabe qual o tipo.
        public List<string> Errors { get; private set; } = new();

    }
}
