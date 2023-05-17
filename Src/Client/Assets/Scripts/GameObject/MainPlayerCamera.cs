using Common;
using Models;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Transform viewPoint;

    public GameObject target;

    private PlayerInputController targetController;

    private bool drag = false;

    [SerializeField]
    private float _mouseSensitivity = 3.0f;

    private float _rotationY;
    private float _rotationX;

    private float _distanceFromTarget;

    private Vector3 _currentRotation;
    private Vector3 _rotationOffset;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.2f;

    [SerializeField]
    private Vector2 _rotationXMinMax = new Vector2(-20, 20);
    private Vector2 _rotationYMinMax = new Vector2(-180, 180);

    public void InitCamera(GameObject target)
    {
        this.target = target;
        this.targetController = target.GetComponent<PlayerInputController>();
        this.transform.position = target.transform.position;
        this.transform.rotation = target.transform.rotation;
        this._rotationOffset = this.transform.localEulerAngles;
        this._distanceFromTarget = Vector3.Distance(this.transform.position, this.target.transform.position);
    }

    private void LateUpdate()
    {
        // 检测点击鼠标右键
        if (Input.GetMouseButton(1))
        {
            if (drag == false)
            {
                drag = true;
            }
        }
        else
        {
            drag = false;
            _rotationY = 0;
            _rotationX = 0;
            _currentRotation = Vector3.zero;
        }

        if (drag && this.target != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            _rotationY += mouseX;
            _rotationX -= mouseY;

            // 限制视角上下移动范围
            _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);
            // 限制视角左右移动范围
            // _rotationY = Mathf.Clamp(_rotationY, _rotationYMinMax.x, _rotationYMinMax.y);

            Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

            // 平滑移动相机
            _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
            transform.localEulerAngles = _currentRotation + _rotationOffset;

            // 保持相机与目标距离
            transform.position = target.transform.position - transform.forward * _distanceFromTarget;
        }
        else
        {
            if (target != null)
            {
                // TODO：仅在角色移动时lerp修改rotation
                this.transform.position = target.transform.position;
                // this.transform.rotation = target.transform.rotation;
                if (targetController != null && targetController.state == SkillBridge.Message.CharacterState.Move)
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, target.transform.rotation, 0.02f);
                this._rotationOffset = this.transform.localEulerAngles;
            }
        }

    }
}
