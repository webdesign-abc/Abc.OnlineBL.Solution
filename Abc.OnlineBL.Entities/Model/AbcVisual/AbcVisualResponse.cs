namespace Abc.OnlineBL.Entities.Model.AbcVisual
{
    using System.Text;
    using System.Runtime.Serialization;

    [DataContract]
    public class AbcVisualResponse
    {
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool OperationHasError { get; set; }
    }
}
