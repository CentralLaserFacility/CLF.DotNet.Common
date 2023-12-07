//
// IDialogHandler.cs
//

namespace Clf.Common.UI
{

  public interface IDialogHandler
  {

    void ShowNameAndDescriptionEditingPanel (
      string                        prompt, 
      string                        initialNameValue, 
      string                        initialDescriptionValue, 
      System.Action<string,string>  okAction, 
      System.Action?                cancelledAction = null
    ) ;

    void ShowTextEntryPanel ( 
      string                 prompt, 
      string                 initialValue, 
      System.Action<string>? okAction        = null, 
      System.Action?         cancelledAction = null
    ) ;

    void ShowOptionSelectionPanel ( 
      string                 prompt, 
      string                 initialValue, 
      System.Action<string>? okAction        = null, 
      System.Action?         cancelledAction = null
    ) ;

    void ShowChannelValueChangeSimulationPanel ( 
      string                 prompt, 
      string                 initialValue, 
      System.Action<string>? okAction        = null, 
      System.Action?         cancelledAction = null
    ) ;

    void ShowMessageBox ( 
      string caption,
      string textLines
    ) ;
    
    bool ShowMessageBox_AskingYesOrNo ( 
      string caption,
      string textLines
    ) ;
    

  }

}
