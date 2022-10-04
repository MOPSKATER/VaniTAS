using System.Reflection;
using UnityEngine;

namespace VaniTAS
{
    internal class CharakterInfo : MonoBehaviour
    {
        private float _hVelocity = 0, _vVelocity = 0, _coyote = 0;
        private readonly FieldInfo _coyoteInfo = RM.drifter.GetType().GetField("jumpForgivenessTimer", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly GUIStyle _hStyle = new()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        private readonly GUIStyle _vStyle = new()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        private readonly GUIStyle _cStyle = new()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        void Update()
        {
            Vector3 velocity = RM.drifter.Motor.BaseVelocity;
            _hVelocity = new Vector2(velocity.x, velocity.z).magnitude;
            _vVelocity = velocity.y;
            _coyote = (float)_coyoteInfo.GetValue(RM.drifter);
            _coyote = Mathf.Floor(Mathf.Max(0f, _coyote) * 1000).Truncate(0);
        }

        void OnGUI()
        {
            if (!RM.mechController.GetIsAlive())
                return;

            Color hColor;
            Color vColor;
            Color cColor = Color.white;

            if (_hVelocity == 0)
                hColor = Color.white;
            else if (_hVelocity < 19)
                hColor = Color.red;
            else
                hColor = Color.green;

            if (_vVelocity < 0)
                vColor = Color.red;
            else if (_vVelocity > 0)
                vColor = Color.green;
            else
                vColor = Color.white;

            if (_coyote != 0)
                cColor = Color.red;

            _hStyle.normal.textColor = hColor;
            _vStyle.normal.textColor = vColor;
            _cStyle.normal.textColor = cColor;

            GUI.Label(new Rect(10, 70, 150, 30), "HVelocity: " + _hVelocity.Truncate(4), _hStyle);
            GUI.Label(new Rect(10, 100, 150, 30), "VVelocity: " + _vVelocity.Truncate(4), _vStyle);
            GUI.Label(new Rect(10, 130, 150, 30), "Coyote: " + _coyote, _cStyle);
        }
    }
}