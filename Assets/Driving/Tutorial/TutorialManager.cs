using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class TutorialManager : MonoBehaviour {
    public float fadeSpeed = 0.1f;


    // Store across levels:
    public static class TutorialManagerInfo {
        public static bool showSpace = false;
        public static bool showArrows = false;
        public static bool showNitro = false;
    }

    protected Image space;
    protected Image arrows;
    protected Image nitro;

    protected Vehicle truck;

    private void Start() {
        space = transform.GetChild(0).GetComponent<Image>();
        arrows = transform.GetChild(1).GetComponent<Image>();
        nitro = transform.GetChild(2).GetComponent<Image>();

        space.color = new Color(1, 1, 1, 0);
        arrows.color = new Color(1, 1, 1, 0);
        nitro.color = new Color(1, 1, 1, 0);

        truck = GetComponentInParent<Vehicle>();
    }

    private delegate bool AwaitTutorialCompletion();

    private IEnumerator ShowTutorialMessage(Image message, AwaitTutorialCompletion check) {
        message.color = Color.white;
        yield return new WaitForSeconds(2f);
        while (check() == false) {
            yield return new WaitForEndOfFrame();
        }
        while (message.color.a > 0) {
            message.color = new Color(1, 1, 1, message.color.a - fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update() {
        if (truck.rb_vehicle.velocity.x <= 0.1f) {
            if (!TutorialManagerInfo.showSpace) {
                TutorialManagerInfo.showSpace = true;
                StartCoroutine(ShowTutorialMessage(space, () => {
                    return Input.GetKeyDown(KeyCode.Space);
                }));
            } else if (!TutorialManagerInfo.showNitro) {
                TutorialManagerInfo.showNitro = true;
                StartCoroutine(ShowTutorialMessage(nitro, () => {
                    return Input.GetKeyDown(KeyCode.LeftShift);
                }));
            }
        }
        if (truck.rb_vehicle.velocity.y > 0 && !TutorialManagerInfo.showArrows) {
            TutorialManagerInfo.showArrows = true;
            StartCoroutine(ShowTutorialMessage(arrows, () => {
                return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
            }));
        }
    }
}
