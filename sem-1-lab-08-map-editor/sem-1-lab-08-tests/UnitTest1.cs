using sem_1_lab_08_map_editor.models;

namespace sem_1_lab_08_tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Restore_ReturnsSavedPlayerPosition()
        {
            var map = Map.Load("map.txt");
            var snapshot = map.CreateSnapshot();
            map.MovePlayer(new Position(2, 2));
            map.Restore(snapshot);
            Assert.That(map.GetPlayerPosition(), Is.EqualTo(snapshot.GetPlayerPosition()));
        }

    }
}
