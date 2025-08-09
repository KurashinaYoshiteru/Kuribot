using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    public GameObject model;    // �A�j���[�V����������3D���f��
    public Animator animator;   // ���f���ɂ��Ă�Animator

    public AnimationClip[] animationClips;

    private string animationName1 = "Animation1";
    private string animationName2 = "Animation2";
    private string animationName3 = "Animation3";
    private string animationName4 = "Animation4";
    private string animationName5 = "Animation5";
    private string animationName6 = "Animation6";


    //�Q�[�����s���Ɉ�x�����Ă΂��֐�
    void Start()
    {
        animator = model.GetComponent<Animator>();  //���f���ɂ��Ă���A�j���[�^�[���擾

        OnUpdateModel();
    }


    //���f����A�j���[�^�[�̏����X�V���鏈��
    public void OnUpdateModel()
    {
        //AnimatorOverrideController��ݒ�
        animator = model.GetComponent<Animator>();
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;


        // ClipPair�̍����ւ������N���b�v�ɑ΂��ď������{��
        AnimationClipPair[] clipPairs = overrideController.clips;
        for (int i = 0; i < overrideController.clips.Length; i++)
        {
            // animationClips��Inspector���ŃA�^�b�`
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
        
        // �����ւ���OverrideController��Animator�ɑ��
        animator.runtimeAnimatorController = overrideController;
    }

    //�{�^���N���b�N���̏���
    public void OnClickButton(string num)
    {
        PlayAnimation(num);
    }

    //�A�j���[�V�����Đ�
    private void PlayAnimation(string num)
    {
        animator.SetTrigger("PlayAnim" + num);
    }
}
