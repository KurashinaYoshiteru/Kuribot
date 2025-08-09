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

    //MixamoAndBipedByNameHumanoidAvatarMapperを参照
    [SerializeField]
    private HumanoidAvatarMapper avatarMapper;

    //ByNameRootBoneMapperを参照
    [SerializeField]
    private RootBoneMapper rootBoneMapper;

    //ModelAnimatorを参照
    [SerializeField]
    private RuntimeAnimatorController modelAnimatorController;

    //インポートしたアニメーション
    //private Animation importAnimation;


    //ゲーム実行時に1度だけ呼ばれる関数
    void Start()
    {
        modelParent = GameObject.Find("ModelParent");
        currentModel = modelParent.transform.GetChild(0).gameObject;
        animationManager = GameObject.Find("AnimationManager");
        animationController = animationManager.GetComponent<AnimationController>();
    }


    //モデルインポートボタンクリック時の処理
    public void OnClickModelImportButton()
    {
        LoadModel();
    }


    //モデル読み込み
    private void LoadModel()
    {
        LoadModelFromExproller();
    }


    //引数に指定したパスからモデルをインポートする（不使用）
    public void LoadModelFromPath(string path)
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();

        // AnimationTypeをHumanoidに設定
        assetLoaderOptions.AnimationType = AnimationType.Humanoid;

        // HumanoidAvatarMapperを設定
        assetLoaderOptions.HumanoidAvatarMapper = avatarMapper;

        AssetLoader.LoadModelFromFile(path, null, OnMaterialsLoad, null, null, null, assetLoaderOptions);
    }


    //エクスプローラーから選択したモデルをインポートする
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


    //ロード後の処理、loadedGameObjectにはロードしたモデルが入る
    private void OnMaterialsLoad(AssetLoaderContext context)
    {
        GameObject loadedGameObject = context.RootGameObject;         //インポートしたモデルの設定
        loadedGameObject.transform.position = new Vector3(0, -1, 0);  //位置調整
        loadedGameObject.transform.SetParent(modelParent.transform);  //ModelParentの子に入れる

        //古いモデルの削除、currentModelの更新
        Destroy(currentModel);
        currentModel = loadedGameObject;


        // Animator経由でAvatarを取得
        var animator = loadedGameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = modelAnimatorController;

        //「AnimationController」オブジェクトにモデルとアニメーターコントローラーを設定
        animationController.model = loadedGameObject;
        animationController.animator = animator;

        animationController.OnUpdateModel();
    }

}
