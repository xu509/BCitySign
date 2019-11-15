
namespace BCity
{

    public class ReqResult
    {
        ResultMessage _resultMessage;
        object _data;

        public ReqResult(ResultMessage resultMessage, object data) {
            _resultMessage = resultMessage;
            _data = data;
        }

        public ReqResult(ResultMessage resultMessage)
        {
            _resultMessage = resultMessage;
        }

        public object GetData() {
            return _data;
        }

    }

}
