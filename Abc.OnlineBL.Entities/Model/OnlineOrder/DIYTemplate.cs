using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Abc.OnlineBL.Entities.Model.OnlineOrder
{
    [DataContract]
    [Serializable]
    public class DIYTemplate
    {
        private int productTemplateId;

        [DataMember]
        public int ProductTemplateId
        {
            get { return productTemplateId; }
            set { productTemplateId = value; }
        }
        private string templateName;

        [DataMember]
        public string TemplateName
        {
            get { return templateName; }
            set { templateName = value; }
        }

        private bool selected;

        [DataMember]
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        [DataMember]
        public string TemplateFormat { get; set; }

        [DataMember]
        public string TemplateDescription { get; set; }

        //[DataMember]
        //public List<LayerContainer> Layers { get; set; }        
    }

    [DataContract]
    [Serializable]
    public class Layer
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        [DataMember]
        public bool Locked { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Group { get; set; }
    }

    [DataContract]
    [Serializable]
    public class LayerContainer
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Layer> Layers { get; set; }
    }
}
