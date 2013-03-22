using System.Windows.Forms;
using mshtml;
class AWBWebBrowser : WebBrowser
{
    public override bool PreProcessMessage(ref Message msg) 
    {
        // look for and intercept a Ctrl+C key up event
        if (msg.Msg == 0x101 && msg.WParam.ToInt32() == (int)Keys.C 
            && ModifierKeys == Keys.Control)
        {
            CopySelectedText();
            return true;
        }
        return base.PreProcessMessage(ref msg);
    }
    
    /// <summary>
    /// Copies the selected text (if any) to the clipboard
    /// </summary>
    public void CopySelectedText()
    {
        Document.ExecCommand("Copy", false, null);
    }
    
    /// <summary>
    /// Returns whether there is currently any text selected
    /// </summary>
    /// <returns></returns>
    public bool TextSelected()
    {        
        IHTMLDocument2 htmlDocument = Document.DomDocument as IHTMLDocument2;

        IHTMLSelectionObject currentSelection= htmlDocument.selection;

        if (currentSelection!=null)
        {
            IHTMLTxtRange range= currentSelection.createRange() as IHTMLTxtRange;

            if (range != null && !string.IsNullOrEmpty(range.text))
                return true;
        }
        
        return false;
    }
}