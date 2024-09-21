using UnityEngine;

public class ShuffleScript : MonoBehaviour
{
    // An array to hold all parking spots (both cars and empty spaces)
    public Transform[] parkingSpots;

    // Function to shuffle the local positions
    public void ShuffleParkingSpots()
    {
        // Create an array to hold the local positions of the parking spots
        Vector3[] originalLocalPositions = new Vector3[parkingSpots.Length];

        // Store each spot's local position
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            originalLocalPositions[i] = parkingSpots[i].localPosition;
        }

        // Shuffle the array of original local positions
        ShuffleArray(originalLocalPositions);

        // Assign the shuffled local positions back to the parking spots
        for (int i = 0; i < parkingSpots.Length; i++)
        {
            parkingSpots[i].localPosition = originalLocalPositions[i];
        }
    }

    // A helper function to shuffle an array (Fisher-Yates shuffle)
    private void ShuffleArray(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    // To test the shuffle in the Unity editor
    private void Update()
    {
        // Press the space bar to shuffle the spots
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShuffleParkingSpots();
        }
    }
}
