using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MapEditorView))]
public class MapEditorController : MonoBehaviour, IUIScreen
{
    public UIScreenType UIScreenType => UIScreenType.Editor;

    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapDataSO config;
    [SerializeField] private CameraController cameraController;
    private enum EditState
    {
        PathEditing,
        ObstacleEditing,
        CoverEditing,
        EnemyEditing,
        PlayerEditing,
        EntityRemoving
    }
    private EditState currentState = EditState.PathEditing;

    private MapEditorView view;
    private UISwitcher uiSwitcher;
    private const TileType DEFAULT_WALKABLE_TYPE = TileType.Path;   //TODO: Add to map config?

    private readonly Dictionary<TileType, TileDataSO> tileDataMap = new();
    private readonly Dictionary<EntityType, EntityDataSO> entityDataMap = new();
    private readonly Dictionary<EditState, TileType> tileEditMap = new()
    {
        {EditState.PathEditing, TileType.Path},
        {EditState.ObstacleEditing, TileType.Obstacle},
        {EditState.CoverEditing, TileType.Cover},
    };

    private (EditState state, ITile tile) lastModified = default;

    void Awake()
    {
        view = GetComponent<MapEditorView>();
        MapTileData();
        MapEntityData();
    }

    void Start()
    {
        ResetConfig();
        SetupView();
        cameraController.SetTargetPos(0, 0);
    }

    private void ResetConfig()
    {
        config.Width = 0;
        config.Height = 0;
    }

    private void MapTileData()
    {
        foreach (var item in config.TileSettings)
        {
            tileDataMap.TryAdd(item.TileData.TileType, item.TileData);
        }
    }

    private void MapEntityData()
    {
        foreach (var item in config.EntitySettings)
        {
            entityDataMap.TryAdd(item.EntityData.EntityType, item.EntityData);
        }
    }

    private void SetupView()    //TODO: make generic? Generate side button only if we have ISideButton attached???
                                // and keep there all data needed? So whenever we add new option, 
                                // there is no need to manually change layout and code, only create new config
    {
        view.AddPathButtonAction(PathAction);
        view.AddObstacleButtonAction(ObstacleAction);
        view.AddCoverButtonAction(CoverAction);
        view.AddEnemyButtonAction(EnemyAction);
        view.AddPlayerButtonAction(PlayerAction);
        view.AddRemoveButtonAction(RemoveAction);

        view.SetRandomizeButtonAction(RandomizeAction);
        view.SetFinishButtonAction(FinishAction);

        view.SetWidthChangeAction(WidthInputAction);
        view.SetHeightChangeAction(HeightInputAction);
    }

    private void PathAction() => currentState = EditState.PathEditing;
    private void ObstacleAction() => currentState = EditState.ObstacleEditing;
    private void CoverAction() => currentState = EditState.CoverEditing;
    private void EnemyAction() => currentState = EditState.EnemyEditing;
    private void PlayerAction() => currentState = EditState.PlayerEditing;
    private void RemoveAction() => currentState = EditState.EntityRemoving;
    private void RandomizeAction() => mapGenerator.GenerateRandom(config);
    private void FinishAction() => uiSwitcher.ShowScreen(UIScreenType.Gameplay);

    private void WidthInputAction(ChangeEvent<int> e)
    {
        if (config.Width == e.newValue) return;

        config.Width = e.newValue;
        view.SetWidth(config.Width);
        InputAction();
    }

    private void HeightInputAction(ChangeEvent<int> e)
    {
        if (config.Height == e.newValue) return;

        config.Height = e.newValue;
        view.SetHeight(config.Height);
        InputAction();
    }

    private void InputAction()
    {
        mapGenerator.GenerateGrid<TileController>(config);
        cameraController.SetTargetPos(config.Width / 2, config.Height / 2);
        cameraController.SetWorldBounds(Vector2.zero, new(config.Width, config.Height));
    }

    private void TileClickAction(TileClickedEvent e)
    {
        if (lastModified.state == currentState && lastModified.tile == e.Tile) return;
        lastModified.state = currentState;
        lastModified.tile = e.Tile;

        if (currentState == EditState.PlayerEditing || currentState == EditState.EnemyEditing)
        {
            tileDataMap.TryGetValue(DEFAULT_WALKABLE_TYPE, out var tileData);
            if (tileData)
            {
                mapGenerator.ChangeTile(e.Tile, tileData);
                mapGenerator.SpawnEntity(GetEntityData(currentState), e.Tile);
            }
        }
        else if (currentState == EditState.EntityRemoving)
        {
            mapGenerator.RemoveEntityAt(e.Tile.Position);
        }
        else
        {
            var targetType = tileEditMap[currentState];
            mapGenerator.ChangeTile(e.Tile, tileDataMap[targetType]);
        }
    }

    private EntityDataSO GetEntityData(EditState state)
    {
        EntityType targetType = EntityType.None;
        if (state == EditState.PlayerEditing) targetType = EntityType.Player;
        else if (state == EditState.EnemyEditing) targetType = EntityType.Enemy;

        entityDataMap.TryGetValue(targetType, out var entityData);
        return entityData;
    }

    public void SetSwitcher(UISwitcher switcher)
    {
        uiSwitcher = switcher;
    }

    public void Show()
    {
        EventBus.Subscribe<TileClickedEvent>(TileClickAction);
        view.Show();
    }

    public void Hide()
    {
        EventBus.Unsubscribe<TileClickedEvent>(TileClickAction);
        mapGenerator.IncreaseMapVersion();
        view.Hide();
    }
}
