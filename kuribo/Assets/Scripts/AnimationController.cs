using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    public GameObject model;    // アニメーションさせる3Dモデル
    public Animator animator;   // モデルについてるAnimator

    public AnimationClip[] animationClips;

    private string animationName1 = "Animation1";
    private string animationName2 = "Animation2";
    private string animationName3 = "Animation3";
    private string animationName4 = "Animation4";
    private string animationName5 = "Animation5";
    private string animationName6 = "Animation6";


    //ゲーム実行時に一度だけ呼ばれる関数
    void Start()
    {
        animator = model.GetComponent<Animator>();  //モデルについているアニメーターを取得

        OnUpdateModel();
    }


    //モデルやアニメーターの情報を更新する処理
    public void OnUpdateModel()
    {
        //AnimatorOverrideControllerを設定
        animator = model.GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;


        // ClipPairの差し替えたいクリップに対して処理を施す
        AnimationClipPair[] clipPairs = overrideController.clips;
        for (int i = 0; i < overrideController.clips.Length; i++)
        {
            // animationClipsはInspector側でアタッチ
            switch (overrideController.clips[i].originalClip.name)
            {
                case "AnimationIdle":
                    clipPairs[i].overrideClip = animationClips[0];
                    break;
                case "Animation1":
                    clipPairs[i].overrideClip = animationClips[1];
                    break;
                case "Animation2":
                    clipPairs[i].overrideClip = animationClips[2];
                    break;
                case "Animation3":
                    clipPairs[i].overrideClip = animationClips[3];
                    break;
                case "Animation4":
                    clipPairs[i].overrideClip = animationClips[4];
                    break;
                case "Animation5":
                    clipPairs[i].overrideClip = animationClips[5];
                    break;
                case "Animation6":
                    clipPairs[i].overrideClip = animationClips[6];
                    break;
                default:
                    break;
            }
        }
        overrideController.clips = clipPairs;
        
        // 差し替えたOverrideControllerをAnimatorに代入
        animator.runtimeAnimatorController = overrideController;
    }

    //ボタンクリック時の処理
    public void OnClickButton(string num)
    {
        PlayAnimation(num);
    }

    //アニメーション再生
    private void PlayAnimation(string num)
    {
        animator.SetTrigger("PlayAnim" + num);
    }
}
