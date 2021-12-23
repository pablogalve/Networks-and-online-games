using UnityEngine;

namespace Mkey
{
    public enum TrackMode { Player, Mouse, Gyroscope, Keyboard, Touch, Auto }
    public enum LayerType { Group, Stars, Planet, Nebula }
    public class CameraFollow : MonoBehaviour
    {
        //player follow options
        private Vector2 margin;
        private Vector2 smooth;

        public TrackMode track = TrackMode.Auto;
        public LayerType type = LayerType.Group;
        public bool ClampPosition;
        public BoxCollider2D ClampField;  // camera motion field

        float nebula_move_speed = 2;
        float group_move_speed = -15;
        float stars_move_speed = 5;

        float group_new_speed_x = 0;
        float group_new_speed_y = 0;


        [SerializeField]
        private GameObject player;
        private Camera m_camera;
        private float camVertSize;
        private float camHorSize;
        private Vector3 acceleration;

        public float ScreenRatio
        {
            get { return ((float)Screen.width / Screen.height); }
        }

        public static CameraFollow Instance;

        #region regular
        void Awake()
        {
            if(!player)  player = GameObject.FindGameObjectWithTag("Player");
            margin = new Vector2(3, 3);
            smooth = new Vector2(1, 1);

            m_camera = GetComponent<Camera>();
            Instance = this;
        }

        private void Start()
        {
            //TouchPad.Instance.ScreenDragEvent += TrackTouchDrag;
        }

        void LateUpdate()
        {
            switch (track)
            {
                case TrackMode.Player:
                    TrackPlayer();
                    break;
                case TrackMode.Keyboard:
                    TrackKeyboard();
                    break;
                case TrackMode.Auto:
                    TrackAuto();
                    break;
            }
        }

        private void OnDesctroy()
        {
           
        }
        #endregion regular

        /// <summary>
        /// Return true if Player out of X margin
        /// </summary>
        private bool OutOfXMargin
        {
            get { return Mathf.Abs(transform.position.x - player.transform.position.x) > margin.x; }
        }

        /// <summary>
        /// Return true if Player out of Y margin
        /// </summary>
        private bool OutOfYMargin
        {
            get
            {
                return Mathf.Abs(transform.position.y - player.transform.position.y) > margin.y;
            }
        }

        private void TrackAuto()
        {
            Vector3 dir = Vector3.zero;
            dir.y = Input.GetAxis("Vertical");
            dir.x = Input.GetAxis("Horizontal");

            if(dir.x>0)
            {
                   group_new_speed_x=dir.x*group_move_speed*7;
            }
            else if(dir.x<0)
            {
                group_new_speed_x = dir.x * group_move_speed *4;
            }
            else
            {
                group_new_speed_x = group_move_speed;
            }

            if (dir.y > 0)
            {
                group_new_speed_y = dir.y * -35;
            }
            else if (dir.y < 0)
            {
                group_new_speed_y = dir.y * -35;
            }
            else
            {
                group_new_speed_y = 0;
            }

            switch (type)
            {
                case LayerType.Group:
                Vector3 target = transform.position + new Vector3(group_new_speed_x, group_new_speed_y, 0);
                transform.position = Vector3.Lerp(transform.position, target, 1.0f * Time.deltaTime);
                    break;

                case LayerType.Stars:
                    Vector3 target2 = transform.position + new Vector3(stars_move_speed, 0, 0);
                    transform.position = Vector3.Lerp(transform.position, target2, 1.0f * Time.deltaTime);
                    break;

                case LayerType.Nebula:
                    Vector3 target3 = transform.position + new Vector3(nebula_move_speed, 0, 0);
                    transform.position = Vector3.Lerp(transform.position, target3, 1.0f * Time.deltaTime);
                    break;

                case LayerType.Planet:
                    Vector3 target4 = transform.position + new Vector3(12, 0, 0);
                    transform.position = Vector3.Lerp(transform.position, target4, 1.0f * Time.deltaTime);
                    break;
            }
            
            ClampCameraPosInField();
        }


        /// <summary>
        /// Clamp camera position in BoxCollider2D rect. Max and Min camera position dependet from collider size, camera size and screen ratio;
        /// </summary>
        private void ClampCameraPosInField()
        {
            if (ClampPosition)
            {
                if (!ClampField) return;

                camVertSize = m_camera.orthographicSize;
                camHorSize = camVertSize * ScreenRatio;

                Vector2 m_bsize = ClampField.bounds.size / 2.0f;
                m_bsize -= new Vector2(camHorSize, camVertSize);

                Vector3 tFieldPosition = ClampField.transform.position;

                float maxY = tFieldPosition.y + m_bsize.y;
                float minY = tFieldPosition.y - m_bsize.y;

                float maxX = tFieldPosition.x + m_bsize.x;
                float minX = tFieldPosition.x - m_bsize.x;

                float posX = Mathf.Clamp(transform.position.x, minX, maxX);
                float posY = Mathf.Clamp(transform.position.y, minY, maxY);

                transform.position = new Vector3(posX, posY, transform.position.z);
            }
        }
        /// <summary>
        /// Camera follow keyboard input
        /// </summary>
        /// <param name="tpea"></param>
        private void TrackKeyboard()
        {
            Vector3 dir = Vector3.zero;
            dir.y = Input.GetAxis("Vertical");
            dir.x = Input.GetAxis("Horizontal");

            Vector3 target = transform.position + new Vector3(dir.x * 10, dir.y * 10, 0);
            transform.position = Vector3.Lerp(transform.position, target, 1.0f * Time.deltaTime);
            ClampCameraPosInField();
        }

        /// <summary>
        /// Camera follow Player Gameobject position
        /// </summary>
        private void TrackPlayer()
        {
            if (!player)
            {
                return;
            }
            float targetX = transform.position.x;
            float targetY = transform.position.y;

            if (OutOfXMargin)
                targetX = Mathf.Lerp(transform.position.x, player.transform.position.x, smooth.x * Time.deltaTime);

            if (OutOfYMargin)
                targetY = Mathf.Lerp(transform.position.y, player.transform.position.y, smooth.y * Time.deltaTime);
            transform.position = new Vector3(targetX, targetY, transform.position.z);
            ClampCameraPosInField();
        }
    }
}