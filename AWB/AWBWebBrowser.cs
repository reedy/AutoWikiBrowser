using System.Windows.Forms;
using mshtml;
using WikiFunctions;
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
    /// Only works if Microsoft.mshtml.dll is available
    /// Check of Globals property must be in separate method to the IHTMLDocument2 code
    /// Further, on some systems even when Assembly can be loaded, it still doesn't work
    /// so require additional try/catch
    /// </summary>
    /// <returns></returns>
    public bool TextSelected()
    {
        if(Globals.MSHTMLAvailable)
        {
            try
            {
                return TextSelectedChecked();
            }
            catch
            {
                // So system reported that Assembly does load, but it still doesn't work
                // See https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_23#Single_click_to_focus_the_edit_box_to_a_line_-_no_longer_works_with_SVN9282
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns whether there is currently any text selected
    /// Only works if Microsoft.mshtml.dll is available
    /// </summary>
    /// <returns></returns>
    private bool TextSelectedChecked()
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