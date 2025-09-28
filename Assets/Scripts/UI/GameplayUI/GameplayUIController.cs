using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(GameplayUIView))]
public class GameplayUIController : MonoBehaviour, IUIScreen
{
    public UIScreenType UIScreenType => UIScreenType.Gameplay;

    [SerializeField] private PathFinder pathFinder;

    private MapDataHandler dataHandler;
    private IEntity targetEntity;
    private GameplayUIView view;
    private UISwitcher uiSwitcher;
    private ITile lastTile;

    private PathData pathData;
    private bool isNewPath = false;

    void Awake()
    {
        view = GetComponent<GameplayUIView>();
    }

    void Start()
    {
        dataHandler = MapDataHandler.Instance;
        SetupView();
    }

    private void SetupView()
    {
        view.SetEditButtonAction(EditAction);
        view.SetConfirmButtonAction(ConfirmAction);

        view.SetAttackRangeChangeAction(AttackRangeInputAction);
        view.SetMoveRangeChangeAction(MoveRangeInputAction);
    }

    public void ChangeEntity(IEntity entity)
    {
        targetEntity = entity;
        if (entity == null)
        {
            view.SetEntityParamsActive(false);
            return;
        }

        view.SetEntityParamsActive(true);
        view.SetAttackRange(entity.AttackRange);
        view.SetMoveRange(entity.MoveRange);
    }

    private void AttackRangeInputAction(ChangeEvent<int> e)
    {
        if (InputBlocker.IsLocked || targetEntity == null || targetEntity.AttackRange == e.newValue)
            return;

        targetEntity.AttackRange = e.newValue;
        view.SetAttackRange(targetEntity.AttackRange);
        pathFinder.ResetPath(pathData);
    }

    private void MoveRangeInputAction(ChangeEvent<int> e)
    {
        if (InputBlocker.IsLocked || targetEntity == null || targetEntity.MoveRange == e.newValue)
            return;

        targetEntity.MoveRange = e.newValue;
        view.SetMoveRange(targetEntity.MoveRange);
        pathFinder.ResetPath(pathData);
    }

    private void EditAction()
    {
        uiSwitcher.ShowScreen(UIScreenType.Editor);
    }

    private void ConfirmAction()
    {
        if (IsValidAction())
        {
            if (isNewPath)
            {
                isNewPath = false;
                (targetEntity as Player).ApplyPath(pathData);
                lastTile = null;
                pathFinder.ShowDestination(pathData);
            }
            else
            {
                pathFinder.ResetPath(pathData);
            }
        }
        else
        {
            //CameraShake;
        }
    }

    private bool IsValidAction()
    {
        return !InputBlocker.IsLocked
            && targetEntity != null
            && targetEntity is Player;
    }

    public void SetSwitcher(UISwitcher switcher)
    {
        uiSwitcher = switcher;
    }

    public void Show()
    {
        EventBus.Subscribe<TileClickedEvent>(TileClickAction);

        var player = dataHandler.GetPlayerEntity();
        ChangeEntity(player);
        if (player != null) pathFinder.UpdateReachablePositions(player.Position);

        view.Show();
    }

    public void Hide()
    {
        EventBus.Unsubscribe<TileClickedEvent>(TileClickAction);
        ChangeEntity(null);
        pathFinder.ResetPath(pathData);
        pathData = null;
        lastTile = null;
        isNewPath = false;
        view.Hide();
    }

    private void TileClickAction(TileClickedEvent e)
    {
        if (lastTile == e.Tile || targetEntity == null) return;
        lastTile = e.Tile;

        pathFinder.ResetPath(pathData);
        if (e.Tile.IsOccupied)
        {
            pathData = pathFinder.FindAttackPath(targetEntity.Position, e.Tile.Position, targetEntity.MoveRange, targetEntity.AttackRange);
        }
        else
        {
            pathData = pathFinder.FindMovePath(targetEntity.Position, e.Tile.Position, targetEntity.MoveRange);
        }
        isNewPath = true;
    }
}
