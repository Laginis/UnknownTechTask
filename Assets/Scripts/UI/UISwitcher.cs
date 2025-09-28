using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] uiScreens;
    [SerializeField] private UIScreenType startScreen;
    private readonly HashSet<IUIScreen> uiScreensMap = new();

    void Awake()
    {
        MapScreens();
    }

    void Start()
    {
        ShowScreen(startScreen);
    }

    public void ShowScreen(UIScreenType screenType)
    {
        foreach (var screen in uiScreensMap)
        {
            if (screen.UIScreenType == screenType) screen.Show();
            else screen.Hide();
        }
    }

    private void MapScreens()
    {
        foreach (var screen in uiScreens)
        {
            if (screen.TryGetComponent(out IUIScreen iScreen))
            {
                uiScreensMap.Add(iScreen);
                iScreen.SetSwitcher(this);
            }
        }
    }
}
