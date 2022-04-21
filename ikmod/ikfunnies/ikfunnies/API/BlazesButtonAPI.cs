using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;
using VRC.UI.Elements.Menus;

/*
    ===============================
    Designed for VRChat Build: 1173
    ===============================
*/

namespace IKMod.API
{
    public class BlazesButtonAPI
    {
        // Replace this with whatever you want. This is to prevent your buttons from overlapping with other mods buttons
        public const string Identifier = "IKMod";

        public static List<QMSingleButton> allQMSingleButtons = new List<QMSingleButton>();
        public static List<QMNestedButton> allQMNestedButtons = new List<QMNestedButton>();
        public static List<QMToggleButton> allQMToggleButtons = new List<QMToggleButton>();
    }

    public class QMButtonBase
    {
        protected GameObject button;
        protected string btnQMLoc;
        protected string btnType;
        protected string btnTag;
        protected int[] initShift = { 0, 0 };
        protected Color OrigBackground;
        protected Color OrigText;
        protected int RandomNumb;

        public GameObject GetGameObject()
        {
            return button;
        }

        public void SetActive(bool state)
        {
            button.gameObject.SetActive(state);
        }

        public void SetLocation(float buttonXLoc, float buttonYLoc)
        {
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.right * (232 * (buttonXLoc + initShift[0]));
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.down * (210 * (buttonYLoc + initShift[1]));

            btnTag = "(" + buttonXLoc + "," + buttonYLoc + ")";
            button.name = btnQMLoc + "/" + btnType + btnTag;
            button.GetComponent<Button>().name = $"{BlazesButtonAPI.Identifier}-{btnType}{btnTag}";
        }

        public void SetToolTip(string buttonToolTip)
        {
            button.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = buttonToolTip;
            button.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_1 = buttonToolTip;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(button);
            }
            catch { }
        }
    }

    public class QMSingleButton : QMButtonBase
    {
        public QMSingleButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null, bool halfBtn = false, Sprite icon = null, bool wingMenu = false)
        {
            btnQMLoc = btnMenu.GetMenuName();
            if (halfBtn)
            {
                btnYLocation -= 0.21f;
            }
            InitButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnTextColor, icon, wingMenu);
            if (halfBtn)
            {
                button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition = new Vector2(0, 22);
            }
        }

        public QMSingleButton(string btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null, bool halfBtn = false, Sprite icon = null, bool wingMenu = false)
        {
            btnQMLoc = btnMenu;
            if (halfBtn)
            {
                btnYLocation -= 0.21f;
            }
            InitButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnTextColor, icon, wingMenu);
            if (halfBtn)
            {
                button.GetComponentInChildren<RectTransform>().sizeDelta /= new Vector2(1f, 2f);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition = new Vector2(0, 22);
            }
        }

        public QMSingleButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null, Sprite icon = null, float btnXSize = default, float btnYSize = default, bool wingMenu = false)
        {
            btnQMLoc = btnMenu.GetMenuName();
            InitButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnTextColor, icon, wingMenu);
            if (btnXSize != default || btnYSize != default)
            {
                button.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(btnXSize, btnYSize);
            }
        }

        private protected void InitButton(float btnXLocation, float btnYLocation, string btnText, Action btnAction, string btnToolTip, Color? btnTextColor = null, Sprite icon = null, bool wingMenu = false)
        {

            btnType = "SingleButton";

            if (!wingMenu)
                button = UnityEngine.Object.Instantiate(APIUtils.SingleButtonTemplate(), APIUtils.FindInactive("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/" + btnQMLoc).transform, true);
            else
                button = UnityEngine.Object.Instantiate(APIUtils.SingleButtonTemplate(), APIUtils.FindInactive(btnQMLoc).transform);


            RandomNumb = APIUtils.RandomNumbers();
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 30;

            button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 176);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-68, 796);

            if (icon == null)
            {
                button.transform.Find("Icon").GetComponentInChildren<Image>().gameObject.SetActive(false);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition += new Vector2(0, 50);
            }
            else
                button.transform.Find("Icon").GetComponentInChildren<Image>().sprite = icon;


            initShift[0] = 0;
            initShift[1] = 0;

            SetLocation(btnXLocation, btnYLocation);
            SetButtonText(btnText);

            SetToolTip(btnToolTip);
            SetAction(btnAction);


            if (btnTextColor != null)
                SetTextColor((Color)btnTextColor);
            else
                OrigText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>().color;


            SetActive(true);
            BlazesButtonAPI.allQMSingleButtons.Add(this);

        }

        public void SetBackgroundImage(Sprite newImg)
        {
            button.transform.Find("Background").GetComponent<Image>().sprite = newImg;
            button.transform.Find("Background").GetComponent<Image>().overrideSprite = newImg;
        }

        public void SetButtonText(string buttonText)
        {
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonText;
        }

        public void SetAction(Action buttonAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            if (buttonAction != null)
                button.GetComponent<Button>().onClick.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityAction>(buttonAction));
        }

        public void ClickMe()
        {
            button.GetComponent<Button>().onClick.Invoke();
        }

        internal void SetTextColor(Color buttonTextColor, bool save = true)
        {
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetOutlineColor(buttonTextColor);
            if (save)
                OrigText = buttonTextColor;
        }
    }

    public class QMToggleButton : QMButtonBase
    {
        protected TextMeshProUGUI btnTextComp;
        protected Button btnComp;
        protected Image btnImageComp;
        protected bool currentState;
        protected Action OnAction;
        protected Action OffAction;

        public QMToggleButton(QMNestedButton location, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string btnToolTip, bool defaultState = false)
        {
            btnQMLoc = location.GetMenuName();
            Initialize(btnXPos, btnYPos, btnText, onAction, offAction, btnToolTip, defaultState);
        }

        public QMToggleButton(string location, float btnXPos, float btnYPos, string btnText, Action onAction, Action offAction, string btnToolTip, bool defaultState = false)
        {
            btnQMLoc = location;
            Initialize(btnXPos, btnYPos, btnText, onAction, offAction, btnToolTip, defaultState);
        }

        private void Initialize(float btnXLocation, float btnYLocation, string btnText, Action onAction, Action offAction, string btnToolTip, bool defaultState)
        {
            btnType = "ToggleButton";
            button = UnityEngine.Object.Instantiate(APIUtils.SingleButtonTemplate(), APIUtils.FindInactive("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/" + btnQMLoc).transform, true);
            RandomNumb = APIUtils.RandomNumbers();
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 176);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(-68, 796);
            btnTextComp = button.GetComponentInChildren<TextMeshProUGUI>(true);
            btnComp = button.GetComponentInChildren<Button>(true);
            btnComp.onClick = new Button.ButtonClickedEvent();
            btnComp.onClick.AddListener(new Action(HandleClick));
            btnImageComp = button.transform.Find("Icon").GetComponentInChildren<Image>(true);

            initShift[0] = 0;
            initShift[1] = 0;
            SetLocation(btnXLocation, btnYLocation);
            SetButtonText(btnText);
            SetButtonActions(onAction, offAction);
            SetToolTip(btnToolTip);
            SetActive(true);

            currentState = defaultState;
            var tmpIcon = currentState ? APIUtils.GetOnIconSprite() : APIUtils.GetOffIconSprite();
            btnImageComp.sprite = tmpIcon;
            btnImageComp.overrideSprite = tmpIcon;

            BlazesButtonAPI.allQMToggleButtons.Add(this);
        }

        private void HandleClick()
        {
            currentState = !currentState;
            var stateIcon = currentState ? APIUtils.GetOnIconSprite() : APIUtils.GetOffIconSprite();
            btnImageComp.sprite = stateIcon;
            btnImageComp.overrideSprite = stateIcon;
            if (currentState)
            {
                OnAction.Invoke();
            }
            else
            {
                OffAction.Invoke();
            }
        }

        public void SetButtonText(string buttonText)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        }

        public void SetButtonActions(Action onAction, Action offAction)
        {
            OnAction = onAction;
            OffAction = offAction;
        }

        public void SetToggleState(bool newState, bool shouldInvoke = false)
        {
            try
            {
                var newIcon = newState ? APIUtils.GetOnIconSprite() : APIUtils.GetOffIconSprite();
                btnImageComp.sprite = newIcon;
                btnImageComp.overrideSprite = newIcon;

                if (shouldInvoke)
                {
                    if (newState)
                    {
                        OnAction.Invoke();
                    }
                    else
                    {
                        OffAction.Invoke();
                    }
                }
            }
            catch { }
        }

        public void ClickMe()
        {
            HandleClick();
        }

        public bool GetCurrentState()
        {
            return currentState;
        }
    }

    public class QMNestedButton
    {
        protected string btnQMLoc;
        protected GameObject MenuObject;
        protected TextMeshProUGUI MenuTitleText;
        protected UIPage MenuPage;
        protected bool IsMenuRoot;
        protected GameObject BackButton;
        protected QMSingleButton MainButton;
        protected string MenuName;

        public QMNestedButton(QMNestedButton location, string btnText, float posX, float posY, string toolTipText, string menuTitle, bool halfButton = false, Sprite icon = null)
        {
            btnQMLoc = location.GetMenuName();
            Initialize(false, btnText, posX, posY, toolTipText, menuTitle, halfButton, icon);
        }

        public QMNestedButton(string location, string btnText, float posX, float posY, string toolTipText, string menuTitle, bool halfButton = false, Sprite icon = null)
        {
            btnQMLoc = location;
            Initialize(location.StartsWith("Menu_"), btnText, posX, posY, toolTipText, menuTitle, halfButton, icon);
        }

        private void Initialize(bool isRoot, string btnText, float btnPosX, float btnPosY, string btnToolTipText, string menuTitle, bool halfButton = false, Sprite icon = null)
        {
            MenuName = $"{BlazesButtonAPI.Identifier}-Menu-{APIUtils.RandomNumbers()}";

            MenuObject = UnityEngine.Object.Instantiate(APIUtils.GetMenuPageTemplate(), APIUtils.GetMenuPageTemplate().transform.parent);

            MenuObject.name = MenuName;

            MenuObject.SetActive(false);

            UnityEngine.Object.DestroyImmediate(MenuObject.GetComponent<LaunchPadQMMenu>());

            MenuPage = MenuObject.AddComponent<UIPage>();


            MenuPage.field_Public_String_0 = MenuName;


            MenuPage.field_Private_Boolean_1 = true;


            MenuPage.field_Protected_MenuStateController_0 = APIUtils.GetMenuStateControllerInstance();


            MenuPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();


            MenuPage.field_Private_List_1_UIPage_0.Add(MenuPage);


            APIUtils.GetMenuStateControllerInstance().field_Private_Dictionary_2_String_UIPage_0.Add(MenuName, MenuPage);



            if (isRoot)
            {
                var list = APIUtils.GetMenuStateControllerInstance().field_Public_ArrayOf_UIPage_0.ToList();
                list.Add(MenuPage);
                APIUtils.GetMenuStateControllerInstance().field_Public_ArrayOf_UIPage_0 = list.ToArray();
            }

            MenuObject.transform.Find("ScrollRect/Viewport/VerticalLayoutGroup").DestroyChildren();
            MenuTitleText = MenuObject.GetComponentInChildren<TextMeshProUGUI>(true);

            MenuTitleText.text = menuTitle;
            MenuTitleText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(95, -50);
            IsMenuRoot = isRoot;
            BackButton = MenuObject.transform.GetChild(0).Find("LeftItemContainer/Button_Back").gameObject;
            BackButton.SetActive(true);
            BackButton.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();

            BackButton.GetComponentInChildren<Button>().onClick.AddListener(new Action(() =>
            {
                if (isRoot)
                {
                    if (btnQMLoc.StartsWith("Menu_"))
                    {
                        APIUtils.GetMenuStateControllerInstance().Method_Public_Void_String_Boolean_0("QuickMenu" + btnQMLoc.Remove(0, 5));
                        return;
                    }
                    APIUtils.GetMenuStateControllerInstance().Method_Public_Void_String_Boolean_0(btnQMLoc);
                    return;
                }
                MenuPage.Method_Protected_Virtual_New_Void_0();
            }));

            MenuObject.transform.GetChild(0).Find("RightItemContainer/Button_QM_Expand").gameObject.SetActive(false);
            MainButton = new QMSingleButton(btnQMLoc, btnPosX, btnPosY, btnText, OpenMe, btnToolTipText, null, halfButton, icon);

            for (int i = 0; i < MenuObject.transform.childCount; i++)
            {
                if (MenuObject.transform.GetChild(i).name != "Header_H1" && MenuObject.transform.GetChild(i).name != "ScrollRect")
                {
                    UnityEngine.Object.Destroy(MenuObject.transform.GetChild(i).gameObject);
                }
            }


            BlazesButtonAPI.allQMNestedButtons.Add(this);
        }

        public void OpenMe()
        {
            APIUtils.GetMenuStateControllerInstance().Method_Public_Void_String_UIContext_Boolean_0(MenuPage.field_Public_String_0);
        }

        public void CloseMe()
        {
            MenuPage.Method_Public_Virtual_New_Void_0();
        }

        public string GetMenuName()
        {
            return MenuName;
        }

        public GameObject GetMenuObject()
        {
            return MenuObject;
        }

        public QMSingleButton GetMainButton()
        {
            return MainButton;
        }

        public GameObject GetBackButton()
        {
            return BackButton;
        }
    }

    public class Tab
    {
        public GameObject gameObject;

        public readonly MenuTab menuTab;

        public readonly Image tabIcon;

        public Tab(QMNestedButton menu, string tooltip, Sprite icon = null)
        {
            gameObject = UnityEngine.Object.Instantiate(APIUtils.GetTabBase(), APIUtils.GetTabBase().transform.parent);
            gameObject.name = menu.GetMenuName() + "Tab";
            tabIcon = gameObject.transform.Find("Icon").GetComponent<Image>();
            tabIcon.sprite = icon;
            tabIcon.overrideSprite = icon;
            menuTab = gameObject.GetComponent<MenuTab>();
            menuTab.field_Private_MenuStateController_0 = APIUtils.GetMenuStateControllerInstance();
            menuTab.field_Public_String_0 = menu.GetMenuName();
            GameObject.Destroy(gameObject.transform.FindChild("Badge"));
            menuTab.gameObject.GetComponent<StyleElement>().field_Private_Selectable_0 = menuTab.gameObject.GetComponent<Button>();
            menuTab.gameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = tooltip;
            menuTab.gameObject.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                menuTab.gameObject.GetComponent<StyleElement>().field_Private_Selectable_0 = menuTab.gameObject.GetComponent<Button>();
            });
        }
    }

    public static class APIUtils
    {
        // Used to make sure that the random number generation per created api item has a new number
        private static readonly System.Random rnd = new System.Random();

        // Cached Instances
        private static VRC.UI.Elements.QuickMenu QuickMenuInstance;
        private static MenuStateController MenuStateControllerInstance;

        // Cached Objects to easily call to at any moment needed to
        private static GameObject SingleButtonReference;
        private static GameObject TabButtonReference;
        private static GameObject MenuPageReference;
        private static GameObject SliderReference;
        private static GameObject SliderLabelReference;
        private static Sprite OnIconReference;
        private static Sprite OffIconReference;

        // Instance Methods
        public static VRC.UI.Elements.QuickMenu GetQuickMenuInstance()
        {
            if (QuickMenuInstance == null)
                QuickMenuInstance = Resources.FindObjectsOfTypeAll<VRC.UI.Elements.QuickMenu>()[0];
            return QuickMenuInstance;
        }

        public static MenuStateController GetMenuStateControllerInstance()
        {
            if (MenuStateControllerInstance == null)
            {
                MenuStateControllerInstance = GetQuickMenuInstance().GetComponent<MenuStateController>();
            }
            return MenuStateControllerInstance;
        }

        // Template Methods
        public static GameObject SingleButtonTemplate()
        {
            if (SingleButtonReference == null)
            {
                var Buttons = GetQuickMenuInstance().GetComponentsInChildren<UnityEngine.UI.Button>(true);
                foreach (var button in Buttons)
                {
                    if (button.name == "Button_Screenshot")
                    {
                        SingleButtonReference = button.gameObject;
                    }
                };
            }
            return SingleButtonReference;
        }

        public static GameObject GetMenuPageTemplate()
        {
            if (MenuPageReference == null)
            {
                MenuPageReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard").gameObject;
            }
            return MenuPageReference;
        }

        public static GameObject GetTabBase()
        {
            if (TabButtonReference == null)
            {
                TabButtonReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_Settings").gameObject;
            }
            return TabButtonReference;
        }

        public static GameObject GetSliderTemplate()
        {
            if (SliderReference == null)
            {
                SliderReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/VolumeSlider").gameObject;
            }
            return SliderReference;
        }

        public static GameObject GetSliderLabelTemplate()
        {
            if (SliderLabelReference == null)
            {
                SliderLabelReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/LevelText").gameObject;
            }
            return SliderLabelReference;
        }

        // Icon Sprite Methods
        public static Sprite GetOnIconSprite()
        {
            if (OnIconReference == null)
            {
                OnIconReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Notifications/Panel_NoNotifications_Message/Icon").GetComponent<UnityEngine.UI.Image>().sprite;
            }
            return OnIconReference;
        }

        public static Sprite GetOffIconSprite()
        {
            if (OffIconReference == null)
            {
                OffIconReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/Buttons_UI_Elements_Row_1/Button_ToggleQMInfo/Icon_Off").GetComponent<UnityEngine.UI.Image>().sprite;
            }
            return OffIconReference;
        }

        // Other Functions
        public static int RandomNumbers()
        {
            return rnd.Next(100000, 999999);
        }

        public static void DestroyChildren(this Transform transform)
        {
            transform.DestroyChildren(null);
        }

        public static void DestroyChildren(this Transform transform, Func<Transform, bool> exclude)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                if (exclude == null || exclude(transform.GetChild(i)))
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }

        public static GameObject FindInactive(string path) // https://github.com/knah
        {
            string[] split = path.Split(new char[] { '/' }, 2);
            Transform rootObject = GameObject.Find($"/{split[0]}")?.transform;

            if (rootObject == null) return null;
            return Transform.FindRelativeTransformWithPath(rootObject, split[1], false)?.gameObject;
        }
    }
}