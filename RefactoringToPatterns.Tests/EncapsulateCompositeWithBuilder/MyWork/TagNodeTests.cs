﻿using NUnit.Framework;
using EncapsulateCompositeWithBuilder.MyWork;

namespace RefactoringToPatterns.EncapsulateCompositeWithBuilder.MyWork
{
    [TestFixture]
    public class TagNodeTests
    {
        private const string SamplePrice = "2.39";

        [Test]
        public void TestSimpleTagWithOneAttributeAndValue()
        {
            var expected =
                "<price currency=" +
                "'" +
                "USD" +
                "'>" +
                SamplePrice +
                "</price>";

            TagNode priceTag = new TagNode("price");
            priceTag.AddAttribute("currency", "USD");
            priceTag.AddValue(SamplePrice);
            Assert.AreEqual(expected, priceTag.ToString());
        }

        [Test]
        public void TestCompositeTagoneChild()
        {
            var expected =
                "<product>" +
                    "<price/>" +
                "</product>"; 

            TagNode productTag = new TagNode("product");
            productTag.Add(new TagNode("price"));

            Assert.AreEqual(expected, productTag.ToString());
        }

        [Test]
        public void TestAddingChildrenAndGrandChildren()
        {
            var expected =
                "<orders>" +
                    "<order>" +
                        "<product/>" +
                    "</order>" +
                "</orders>";

            TagNode ordersTag = new TagNode("orders");
            TagNode orderTag = new TagNode("order");
            orderTag.Add(new TagNode("product"));
            ordersTag.Add(orderTag);

            Assert.AreEqual(expected, ordersTag.ToString());
        }

        [Test]
        public void TestSelfClosingSingularTag()
        {
            var expected = "<flavors/>";

            TagNode flavorsTag = new TagNode("flavors");

            Assert.AreEqual(expected, flavorsTag.ToString());
        }

        [Test]
        public void TestParents()
        {
            TagNode root = new TagNode("root");
            Assert.Null(root.Parent);

            TagNode childNode = new TagNode("child");
            root.Add(childNode);
            Assert.AreEqual(root, childNode.Parent);
            Assert.AreEqual("root", childNode.Parent.Name);
        }
    }
}
