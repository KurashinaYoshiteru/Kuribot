using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore;
using TriLibCore.General;
using TriLibCore.Mappers;
using TriLibCore.Extensions;


public class ImportController : MonoBehaviour
{
    private GameObject modelParent;
    private GameObject currentModel;
    private GameObject animationManager;
    private AnimationController animationController;

    //MixamoAndBipedByNameHumanoidAvatarMapper���Q��
    [SerializeField]
    private HumanoidAvatarMapper avatarMapper;

    //ByNameRootBoneMapper���Q��
    [SerializeField]
    private RootBoneMapper rootBoneMapper;

    //ModelAnimator���Q��
    [SerializeField]
    private RuntimeAnimatorController modelAnimatorController;

    //�C���|�[�g�����A�j���[�V����
    //private Animation importAnimation;


    //�Q�[�����s����1�x�����Ă΂��֐�
    void Start()
    {
        modelParent = GameObject.Find("ModelParent");
        currentModel = modelParent.transform.GetChild(0).gameObject;
        animationManager = GameObject.Find("AnimationManager");
        animationController = animationManager.GetComponent<AnimationController>();
    }


    //���f���C���|�[�g�{�^���N���b�N���̏���
    public void OnClickModelImportButton()
    {
        LoadModel();
    }


    //���f���ǂݍ���
    private void LoadModel()
    {
        LoadModelFromExproller();
    }


    //�����Ɏw�肵���p�X���烂�f�����C���|�[�g����i�s�g�p�j
    public void LoadModelFromPath(string path)
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();

        // AnimationType��Humanoid�ɐݒ�
        assetLoaderOptions.AnimationType = AnimationType.Humanoid;

        // HumanoidAvatarMapper��ݒ�
        assetLoaderOptions.HumanoidAvatarMapper = avatarMapper;

        AssetLoader.LoadModelFromFile(path, null, OnMaterialsLoad, null, null, null, assetLoaderOptions);
    }


    //�G�N�X�v���[���[����I���������f�����C���|�[�g����
    public void LoadModelFromExproller()
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        assetLoaderOptions.AnimationType = AnimationType.Humanoid;
        assetLoaderOptions.AvatarDefinition = AvatarDefinitionType.CreateFromThisModel;
        assetLoaderOptions.HumanoidAvatarMapper = avatarMapper;
        assetLoaderOptions.RootBoneMapper = rootBoneMapper;


        var assetLoaderFilePicker = AssetLoaderFilePicker.Create();
        assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a File", null, OnMaterialsLoad, null, null, null, null, assetLoaderOptions);
    }


    //���[�h��̏����AloadedGameObject�ɂ̓��[�h�������f��������
    private void OnMaterialsLoad(AssetLoaderContext context)
    {
        GameObject loadedGameObject = context.RootGameObject;         //�C���|�[�g�������f���̐ݒ�
        loadedGameObject.transform.position = new Vector3(0, -1, 0);  //�ʒu����
        loadedGameObject.transform.SetParent(modelParent.transform);  //ModelParent�̎q�ɓ����

        //�Â����f���̍폜�AcurrentModel�̍X�V
        Destroy(currentModel);
        currentModel = loadedGameObject;


        // Animator�o�R��Avatar���擾
        var animator = loadedGameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = modelAnimatorController;

        //�uAnimationController�v�I�u�W�F�N�g�Ƀ��f���ƃA�j���[�^�[�R���g���[���[��ݒ�
        animationController.model = loadedGameObject;
        animationController.animator = animator;

        animationController.OnUpdateModel();
    }

}
