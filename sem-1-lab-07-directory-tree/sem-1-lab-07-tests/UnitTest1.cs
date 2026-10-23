using sem_1_lab_07_directory_tree;
using System.Reflection.Emit;

namespace sem_1_lab_07_tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BuildTree_PreservesNesting()
        {
            var tree = TreeBuilder.Build(File.ReadAllLines("tree.txt"));
            Assert.That(tree.Children[0].Children[0].Name, Is.EqualTo("C"));
        }
    }
}
