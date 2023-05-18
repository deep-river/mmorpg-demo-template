using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UIPausePanel : UIWindow
{
    public void ExitToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void ExitGame()
    {
        Services.UserService.Instance.SendGameLeave(true);
    }
}