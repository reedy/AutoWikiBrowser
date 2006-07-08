using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace WikiFunctions.MWB
{

class RegexXml
{
  public const string Name = "regex";
  public static void Write(XmlWriter w, RegexOptions o)
  {
    w.WriteStartElement(Name);
    if (o != RegexOptions.None)
      w.WriteAttributeString("options", ((int)o).ToString());
    w.WriteEndElement();
  }
  public static RegexOptions Read(XmlTextReader rd)
  {
    RegexOptions res = RegexOptions.None;
    if (rd.Name == Name) 
    {
      if (rd.MoveToAttribute("options"))
        res = (RegexOptions) Convert.ToInt32(rd.Value);
    }
    return res;
  }
}

}
