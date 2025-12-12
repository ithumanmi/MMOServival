using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.GameFlow
{
    public partial class RDM : RuntimeSingleton<RDM>
    {
        public TutorialPopupModel tutorialPopupModel;
    }

    public class TutorialPopupModel
    {
        public List<TutorialPopupModelUnit> units = new List<TutorialPopupModelUnit>();

        public int returned;
        public float backgroundAlpha = 200f / 256f;
        public class TutorialPopupModelUnit
        {
            public Vector2 screenPoint;
            public Vector2 size;
            public Action onClick;
            public bool showHand = true;

            // text
            public string content;
            public Vector2 contentScreenPoint = new Vector2(0, 200f);
            public Vector2 contentPivot = new Vector2(0.5f, 0);
            public Vector2 contentSize = new Vector2(500f, 200);

            public ButtonType buttonType;
        }

        public enum ButtonType
        {
            // nhấn vào bất kì
            Main = 0,

            // nhấn vào đúng vùng sáng mới được tiếp tục
            Focus = 1,
        }
    }
}
