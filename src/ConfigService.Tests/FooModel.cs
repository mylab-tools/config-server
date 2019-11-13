using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigService.Tests
{
    public class FooModel
    {
        public string FooParam { get; set; }
        public string ParamForOverride { get; set; }
        public string SecretParam { get; set; }

        public InnerModel InnerObject { get; set; }
    }

    public class InnerModel
    {
        public string Bar { get; set; }
    }
}
