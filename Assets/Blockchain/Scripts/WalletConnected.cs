using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace DD.Web3
{
    public class WalletConnected : MonoBehaviour
    {
        [SerializeField] private List<GameObject> gameObjetsToActive = new List<GameObject>();

        [SerializeField] private List<GameObject> gameObjetsToDeactive = new List<GameObject>();




        [SerializeField] private bool willLoadScene = false;
        [SerializeField] private int sceneIndexToLoad = 1;

        public async void OnWalletConnected()
        {
            foreach (GameObject objectToActive in gameObjetsToActive)
            {
                objectToActive.SetActive(true);
            }
            foreach (GameObject objectToDeactive in gameObjetsToDeactive)
            {
                objectToDeactive.SetActive(false);
            }


            // START GAME
            //GameController.Instance.StartGame();

            if (willLoadScene)
            {
                await SceneManager.LoadSceneAsync(sceneIndexToLoad);
            }

             //LocalStorageManager.Instance.FetchUserData();


            BlockchainManager.Instance.connectionManager.ShowLoadingScreen(false);


        }




    }
}

