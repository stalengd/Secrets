using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Anomalus.Items.UI
{
    public class InventoryUICell : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] protected Image BodyImage;
        [SerializeField] protected ItemRenderer Renderer;
        [SerializeField] protected GameObject DragRendererPrefab;

        //[Header("Audio")]
        //[SerializeField] private SharedSoundsSource.RandomPitchPlayback moveAudio;

        public InventoryUI Inventory { get; set; }
        public int? Index { get; set; }
        public Inventory.Slot Stack { get; private set; }
        public bool IsDraggable { get; set; }
        public bool IsDragCopy { get; private set; }

        private ItemRenderer _draggableCopy;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = transform as RectTransform;

                return _rectTransform;
            }
        }
        private RectTransform _rectTransform;

        private void OnDisable()
        {
            CancelDrag();
        }

        public void SetState(Inventory.Slot stack)
        {
            Stack = stack;

            CancelDrag(false);

            if (stack != null)
            {
                Renderer.Render(stack.AsStack);
            }
            else
            {
                Renderer.Render(null);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Stack == null) return;
            if (!Inventory.Screen.IsItemsInteractable) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Inventory.Screen.MoveStackInput.action.IsPressed())
                {
                    if (Inventory.InventoryOwner.MoveStackToOtherInventory(Stack))
                        ;
                        //SharedSoundsSource.Play(moveAudio);
                }
                else if (Inventory.Screen.SplitStackInput.action.IsPressed())
                {
                    if (Inventory.InventoryOwner.MoveStackToOtherInventory(Stack, Mathf.Max(Stack.Count / 2, 1)))
                        ;
                        //SharedSoundsSource.Play(moveAudio);
                }
                else if (eventData.clickCount >= 2)
                {
                    if (Inventory.InventoryOwner.MoveStackToOtherInventory(Stack, 1))
                        ;
                        //SharedSoundsSource.Play(moveAudio);
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (Stack == null) return;
            if (!IsDraggable || !Inventory.Screen.IsItemsInteractable) return;

            var prefab = DragRendererPrefab;
            if (prefab == null) prefab = gameObject;

            _draggableCopy = Inventory.Screen.CreateStackCopyOnTopLayer(this, prefab);
            var rt = _draggableCopy.transform as RectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.sizeDelta = (transform as RectTransform).rect.size;
            //draggableCopy.IsDragCopy = true;

            //draggableCopy.bodyImage.color = dragTint;
            Renderer.Render(null);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (Stack == null) return;
            if (!IsDraggable || !Inventory.Screen.IsItemsInteractable) return;

            var dropTarget = Inventory.Screen.GetInventoryOverMouse();

            if (dropTarget != null)
            {
                var r = dropTarget.DropItem(Stack);

                if (r)
                {
                    //SharedSoundsSource.Play(moveAudio);
                }
            }
            else
            {
                Inventory.InventoryOwner.DropItem(Stack.AsStack);
                Stack.Inventory.RemoveItem(Stack);
            }

            CancelDrag();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Stack == null) return;
            if (!IsDraggable || !Inventory.Screen.IsItemsInteractable) return;

            _draggableCopy.transform.position = Input.mousePosition;
        }

        public void OnInventoryClosed()
        {
            CancelDrag();
        }

        public bool IsMouseOver()
        {
            if (!gameObject.activeInHierarchy) return false;

            var boundsRect = transform as RectTransform;
            var scale = boundsRect.lossyScale.x;
            var rect = boundsRect.rect;
            rect = new Rect(rect.position * scale, rect.size * scale);
            var bounds = new Bounds((Vector3)rect.center + boundsRect.position, (Vector3)rect.size + Vector3.forward * 100f);
            return bounds.Contains(Input.mousePosition);
        }

        private void CancelDrag(bool rerender = true)
        {
            if (_draggableCopy == null)
                return;

            Destroy(_draggableCopy.gameObject);
            if (rerender && Stack != null)
                Renderer.Render(Stack.AsStack);
        }
    }
}
