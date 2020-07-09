using System.Collections.Generic;

namespace form_builder.Configuration
{
    public class NotifySmsConfiguration
    {
        public List<Template> Templates { get; set; }
    }

    public class Template
    {
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
