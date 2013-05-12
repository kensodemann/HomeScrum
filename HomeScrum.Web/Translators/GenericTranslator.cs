using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class GenericTranslator<SourceT, TargetT> : PropertyNameTranslator<SourceT, TargetT> { }
}