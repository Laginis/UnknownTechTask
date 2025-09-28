using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIView : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    private VisualElement root;

    private Button editButton;
    private Button confirmButton;

    private VisualElement attackRange;
    private VisualElement moveRange;

    private IntegerField attackRangeInput;
    private IntegerField moveRangeInput;

    private const string EDIT_BUTTON_NAME = "EditBtn";
    private const string CONFIRM_BUTTON_NAME = "ConfirmBtn";

    private const string ATTACK_RANGE_NAME = "AttackRange";
    private const string MOVE_RANGE_NAME = "MoveRange";

    private const string ATTACK_RANGE_INPUT_NAME = "AttackRangeInput";
    private const string MOVE_RANGE_INPUT_NAME = "MoveRangeInput";

    private List<VisualElement> entityParams;

    void Awake()
    {
        root = uIDocument.rootVisualElement;
        FindElements();
    }

    private void FindElements()
    {
        editButton = root.Q<Button>(EDIT_BUTTON_NAME);
        confirmButton = root.Q<Button>(CONFIRM_BUTTON_NAME);

        attackRange = root.Q<VisualElement>(ATTACK_RANGE_NAME);
        moveRange = root.Q<VisualElement>(MOVE_RANGE_NAME);
        entityParams = new() { attackRange, moveRange };

        attackRangeInput = root.Q<IntegerField>(ATTACK_RANGE_INPUT_NAME);
        moveRangeInput = root.Q<IntegerField>(MOVE_RANGE_INPUT_NAME);
    }

    public void SetEditButtonAction(Action action) => editButton.clicked += action;
    public void SetConfirmButtonAction(Action action) => confirmButton.clicked += action;

    public void SetAttackRange(int attackRange) => attackRangeInput.value = attackRange;
    public void SetMoveRange(int moveRange) => moveRangeInput.value = moveRange;

    public void SetAttackRangeChangeAction(EventCallback<ChangeEvent<int>> callback)
    {
        attackRangeInput.RegisterValueChangedCallback(callback);
    }

    public void SetMoveRangeChangeAction(EventCallback<ChangeEvent<int>> callback)
    {
        moveRangeInput.RegisterValueChangedCallback(callback);
    }

    public void SetEntityParamsActive(bool isActive)
    {
        var visibility = isActive ? Visibility.Visible : Visibility.Hidden;
        foreach (var param in entityParams)
        {
            param.style.visibility = visibility;
        }
    }

    public void Show()
    {
        UnfocusFields(attackRangeInput);
        UnfocusFields(moveRangeInput);
        root.style.display = DisplayStyle.Flex;

    }
    public void Hide()
    {
        UnfocusFields(attackRangeInput);
        UnfocusFields(moveRangeInput);
        root.style.display = DisplayStyle.None;
    }

    private void UnfocusFields(IntegerField field) => field.Blur();
}
