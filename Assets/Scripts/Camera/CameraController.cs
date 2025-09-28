using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private AnimationCurve moveSpeedZoomCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Space(10)]
    [SerializeField] private float sprintMultiplier = 3f;

    [Space(10)]
    [SerializeField] private float edgeScrollingMargin = 15f;

    private Vector2 edgeScrollingInput;
    private Vector3 velocity = Vector3.zero;
    private float decelerationMultiplier = 1f;

    [Header("Orbit")]
    [SerializeField] private float orbitSensitivity = 0.5f;
    [SerializeField] private float orbitSmoothing = 5f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float zoomSmoothing = 5f;
    private float currentZoomSpeed = 0f;

    public float ZoomLevel  //value between 0 (zoomed in) and 1 (zoomed out)
    {
        get
        {
            InputAxis axis = orbitalFollow.RadialAxis;
            return Mathf.InverseLerp(axis.Range.x, axis.Range.y, axis.Value);
        }
    }

    [Header("Components")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    private Camera cam;
    private Vector2 moveInput;
    private Vector2 scrollInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool middleClickInput = false;

    private Vector3 inputVector = new();
    private Vector2 minBounds = new();
    private Vector2 maxBounds = new();

    void Start()
    {
        cam = Camera.main;
    }

    //From PlayerInput component
    private void OnSprint(InputValue value) => sprintInput = value.isPressed;
    private void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    private void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    private void OnScrollWheel(InputValue value) => scrollInput = value.Get<Vector2>();
    private void OnMiddleClick(InputValue value) => middleClickInput = value.isPressed;

    void LateUpdate()
    {
        float deltaTime = Time.unscaledDeltaTime;

        if (!Application.isEditor) UpdateEdgeScrolling();

        UpdateOrbit(deltaTime);
        UpdateMovement(deltaTime);
        UpdateZoom(deltaTime);
    }

    private void UpdateMovement(float deltaTime)
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        inputVector.Set(moveInput.x + edgeScrollingInput.x, 0, moveInput.y + edgeScrollingInput.y);
        if (inputVector.sqrMagnitude > 1f) inputVector.Normalize();

        float zoomMultiplier = moveSpeedZoomCurve.Evaluate(ZoomLevel);
        float sprintFactor = sprintInput ? sprintMultiplier : 1f;

        Vector3 targetVelocity = moveSpeed * zoomMultiplier * sprintFactor * inputVector;

        if (inputVector.sqrMagnitude > 0.01f)
        {
            velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * sprintFactor * deltaTime);
            if (sprintInput) decelerationMultiplier = sprintMultiplier;
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * decelerationMultiplier * deltaTime);
            if (velocity.sqrMagnitude <= 0.01f) decelerationMultiplier = 1f;
        }

        cameraTarget.position += (deltaTime * velocity.z * forward) + (deltaTime * velocity.x * right);
        //TODO: clamp to world bounds
    }

    private void UpdateOrbit(float deltaTime)
    {
        if (!middleClickInput || (lookInput.sqrMagnitude <= 0.01f)) return;

        float sens = orbitSensitivity * orbitSmoothing * deltaTime;
        InputAxis horizontalAxis = orbitalFollow.HorizontalAxis;
        InputAxis verticalAxis = orbitalFollow.VerticalAxis;

        horizontalAxis.Value += lookInput.x * sens;
        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value - lookInput.y * sens, verticalAxis.Range.x, verticalAxis.Range.y);

        orbitalFollow.HorizontalAxis = horizontalAxis;
        orbitalFollow.VerticalAxis = verticalAxis;
    }

    private void UpdateZoom(float deltaTime)
    {
        InputAxis axis = orbitalFollow.RadialAxis;

        float targetZoomSpeed = scrollInput.y != 0f ? zoomSpeed * scrollInput.y : 0f;
        currentZoomSpeed = Mathf.Lerp(currentZoomSpeed, targetZoomSpeed, zoomSmoothing * deltaTime);

        axis.Value = Mathf.Clamp(axis.Value - currentZoomSpeed, axis.Range.x, axis.Range.y);
        orbitalFollow.RadialAxis = axis;
    }

    private void UpdateEdgeScrolling()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        edgeScrollingInput = Vector2.zero;

        if (mousePosition.x <= edgeScrollingMargin)
            edgeScrollingInput.x = -1f;
        else if (mousePosition.x >= Screen.width - edgeScrollingMargin)
            edgeScrollingInput.x = 1f;

        if (mousePosition.y <= edgeScrollingMargin)
            edgeScrollingInput.y = -1f;
        else if (mousePosition.y >= Screen.height - edgeScrollingMargin)
            edgeScrollingInput.y = 1f;
    }

    public void SetTargetPos(float x, float z)
    {
        cameraTarget.position = new(x, cameraTarget.position.y, z);
    }

    public void SetWorldBounds(Vector2 min, Vector2 max) //TODO: add usage
    {
        minBounds = min;
        maxBounds = max;
    }
}
