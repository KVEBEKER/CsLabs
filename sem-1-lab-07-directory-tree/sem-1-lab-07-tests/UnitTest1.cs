using sem_1_lab_07_directory_tree;
using System.Reflection.Emit;

namespace sem_1_lab_07_tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            TreeBuilder.ReadTreeText("tree.txt");
        }

        [Test]
        public void BuildTree_CountReadingLines()
        {
            var treePassedLines = TreeBuilder.PrintTree(0);
            Assert.That(treePassedLines, Is.EqualTo(9));
        }
    }
}
