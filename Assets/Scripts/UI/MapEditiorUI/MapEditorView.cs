using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditorView : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    private VisualElement root;

    private Button pathButton;
    private Button obstacleButton;
    private Button coverButton;
    private Button enemyButton;
    private Button playerButton;
    private Button removeButton;

    private Button randomizeButton;
    private Button finishButton;

    private IntegerField widthInput;
    private IntegerField heightInput;

    private const string PATH_BUTTON_NAME = "PathBtn";
    private const string OBSTACLE_BUTTON_NAME = "ObstacleBtn";
    private const string COVER_BUTTON_NAME = "CoverBtn";
    private const string ENEMY_BUTTON_NAME = "EnemyBtn";
    private const string PLAYER_BUTTON_NAME = "PlayerBtn";
    private const string REMOVE_BUTTON_NAME = "RemoveBtn";

    private const string RANDOMIZE_BUTTON_NAME = "RandomizeBtn";
    private const string FINISH_BUTTON_NAME = "FinishBtn";

    private const string WIDTH_INPUT_NAME = "WidthInput";
    private const string HEIGHT_INPUT_NAME = "HeightInput";

    private const string ACTIVE_CLASS = "active";

    private readonly List<Button> sideButtons = new();


    void Awake()
    {
        root = uIDocument.rootVisualElement;
        FindElements();
        SetupSideButtons();
    }

    private void FindElements() //TODO: make generic? Generate side button only if we have ISideButton attached???
                                // and keep there all data needed? So whenever we add new option, 
                                // there is no need to manually change layout and code, only create new config
    {
        pathButton = root.Q<Button>(PATH_BUTTON_NAME);
        obstacleButton = root.Q<Button>(OBSTACLE_BUTTON_NAME);
        coverButton = root.Q<Button>(COVER_BUTTON_NAME);
        enemyButton = root.Q<Button>(ENEMY_BUTTON_NAME);
        playerButton = root.Q<Button>(PLAYER_BUTTON_NAME);
        removeButton = root.Q<Button>(REMOVE_BUTTON_NAME);

        sideButtons.AddRange(new[] {
            pathButton,
            obstacleButton,
            coverButton,
            enemyButton,
            playerButton,
            removeButton
        });

        randomizeButton = root.Q<Button>(RANDOMIZE_BUTTON_NAME);
        finishButton = root.Q<Button>(FINISH_BUTTON_NAME);

        widthInput = root.Q<IntegerField>(WIDTH_INPUT_NAME);
        heightInput = root.Q<IntegerField>(HEIGHT_INPUT_NAME);
    }

    private void SetupSideButtons()
    {
        foreach (var button in sideButtons)
        {
            button.clicked += () => UpdateSideButtons(button);
        }

        UpdateSideButtons(pathButton);
    }

    private void UpdateSideButtons(Button newActiveButton)
    {
        foreach (var button in sideButtons)
        {
            if (button == newActiveButton) button.AddToClassList(ACTIVE_CLASS);
            else button.RemoveFromClassList(ACTIVE_CLASS);
        }
    }

    public void AddPathButtonAction(Action action) => pathButton.clicked += action;
    public void AddObstacleButtonAction(Action action) => obstacleButton.clicked += action;
    public void AddCoverButtonAction(Action action) => coverButton.clicked += action;
    public void AddEnemyButtonAction(Action action) => enemyButton.clicked += action;
    public void AddPlayerButtonAction(Action action) => playerButton.clicked += action;
    public void AddRemoveButtonAction(Action action) => removeButton.clicked += action;

    public void SetRandomizeButtonAction(Action action) => randomizeButton.clicked += action;
    public void SetFinishButtonAction(Action action) => finishButton.clicked += action;

    public void SetWidth(int width) => widthInput.value = width;
    public void SetHeight(int height) => heightInput.value = height;

    public void SetWidthChangeAction(EventCallback<ChangeEvent<int>> callback)
    {
        widthInput.RegisterValueChangedCallback(callback);
    }

    public void SetHeightChangeAction(EventCallback<ChangeEvent<int>> callback)
    {
        heightInput.RegisterValueChangedCallback(callback);
    }

    public void Show() => root.style.display = DisplayStyle.Flex;
    public void Hide() => root.style.display = DisplayStyle.None;
}
