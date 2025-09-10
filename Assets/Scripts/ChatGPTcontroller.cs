using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Extensions;

namespace OpenAI
{
    public class ChatGPTcontroller : MonoBehaviour
    {
        [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private float height;
        public void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        public void AppendMessage(ChatMessage message, Color color)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.GetChild(0).GetComponent<Image>().color = color;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        /// <summary>
        /// Activa o desactiva el objeto "ChatWait" y actualiza su animación.
        /// </summary>
        /// <param name="status">Indica si debe estar activo (true) o inactivo (false).</param>
        public void StartChatWait(bool status)
        {
            try
            {
                // Buscar el objeto "ChatWait" dentro de la jerarquía.
                GameObject chatWait = transform.Find("ChatWait").gameObject;

                // Activar o desactivar el objeto.
                chatWait.SetActive(status);

                // Actualizar el estado del parámetro "status" en el Animator.
                Animator animator = chatWait.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("status", status);
                }
                else
                {
                    Debug.LogWarning("Animator no encontrado en el objeto 'ChatWait'.");
                }
            }
            catch (Exception ex)
            {
                // Registrar la excepción con más contexto.
                Debug.LogException(ex);
            }
        }

        public void ClearContent()
        {
            foreach (Transform child in scroll.content)
            {
                Destroy(child.gameObject);
            }
        }

    }
}

