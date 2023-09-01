using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    public GameObject obstaclePrefab;
    public GameObject moneyPrefab;

    public float massIncreaseAmount = 1;
    public Transform spawnPoint; // Точка спавна

    private IEnumerator SpawnObjects ( ) {
        while ( true ) {
            GameObject chosenPrefab = Random.Range (0, 2) == 0 ? obstaclePrefab : moneyPrefab;

            SpawnObject (chosenPrefab, spawnPoint);

            // Ждем перед следующим спауном
            float spawnDelay = Random.Range (0.5f, 1f);
            yield return new WaitForSeconds (spawnDelay);
        }
    }

    private void SpawnObject ( GameObject prefab, Transform spawnPoint ) {
        Vector3 spawnPosition = new Vector3 (
            Random.Range (spawnPoint.position.x - 2f, spawnPoint.position.x + 2f),
            spawnPoint.position.y,
            Random.Range (spawnPoint.position.z - 2f, spawnPoint.position.z + 2f)
        );

        GameObject spawnedObject = Instantiate (prefab, spawnPosition, Quaternion.Euler (0f, 0f, -180f));
        Rigidbody objectRigidbody = spawnedObject.GetComponent<Rigidbody> ();
        if ( objectRigidbody != null ) {
            objectRigidbody.mass += massIncreaseAmount; // Увеличиваем массу
        }
    }

    public void IncreaseMass ( ) => massIncreaseAmount += 0;
}
