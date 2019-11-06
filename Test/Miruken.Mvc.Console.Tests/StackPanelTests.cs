namespace Miruken.Mvc.Console.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StackPanelTests: TestBase
    {
        private readonly char[][] a = {
            new[] {'a', 'a', 'a', 'a', 'a'},
            new[] {'a', 'a', 'a', 'a', 'a'},
            new[] {'a', 'a', 'a', 'a', 'a'},
            new[] {'a', 'a', 'a', 'a', 'a'},
            new[] {'a', 'a', 'a', 'a', 'a'}
        };

        private readonly char[][] b = {
            new[] {'b', 'b', 'b', 'b', 'b'},
            new[] {'b', 'b', 'b', 'b', 'b'},
            new[] {'b', 'b', 'b', 'b', 'b'},
            new[] {'b', 'b', 'b', 'b', 'b'},
            new[] {'b', 'b', 'b', 'b', 'b'}
        };

        [TestMethod]
        public void CanAddAndRemoveChildren()
        {
            var main = new StackPanel()
            {
                Size = new Size(5, 5)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));
            AssertCellsAreEquivelant(a, Render(main.Size, main));

            main.Add(new Buffer().Wrap(new string('b', 25)));
            AssertCellsAreEquivelant(b, Render(main.Size, main));

            main.RemoveLast();
            AssertCellsAreEquivelant(a, Render(main.Size, main));
        }
    }

    [TestClass]
    public class StackPanelHorizontalAlignment : TestBase
    {
        [TestMethod]
        public void Left()
        {
            Assert(HorizontalAlignment.Left, new []
            {
                new []{'-','-','-','a','a'},
                new []{'|','z','|','a','a'},
                new []{'|',' ','|','a','a'},
                new []{'|',' ','|','a','a'},
                new []{'-','-','-','a','a'}
            });
        }

        [TestMethod]
        public void Center()
        {
            Assert(HorizontalAlignment.Center, new []
            {
                new []{'a','-','-','-','a'},
                new []{'a','|','z','|','a'},
                new []{'a','|',' ','|','a'},
                new []{'a','|',' ','|','a'},
                new []{'a','-','-','-','a'}
            });
        }

        [TestMethod]
        public void Right()
        {
            Assert(HorizontalAlignment.Right, new []
            {
                new []{'a','a','-','-','-'},
                new []{'a','a','|','z','|'},
                new []{'a','a','|',' ','|'},
                new []{'a','a','|',' ','|'},
                new []{'a','a','-','-','-'}
            });
        }

        [TestMethod]
        public void Stretch()
        {
            Assert(HorizontalAlignment.Stretch, new []
            {
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'-','-','-','-','-'}
            });
        }

        [TestMethod]
        public void Unknown()
        {
            Assert(HorizontalAlignment.Unknown, new []
            {
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'-','-','-','-','-'}
            });
        }

        public void Assert (HorizontalAlignment horizontalAlignment, char[][] expected)
        {
            var main = new StackPanel()
            {
                Size = new Size(5, 5)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));

            var modal = new Buffer
            {
                Border = new Thickness(1),
                HorizontalAlignment = horizontalAlignment,
                Size = new Size(3, 3)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(expected, Render(main.Size, main));
        }
    }

    [TestClass]
    public class StackPanelVerticalAlignment : TestBase
    {
        [TestMethod]
        public void Top()
        {
            Assert(VerticalAlignment.Top, new []
            {
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'-','-','-','-','-'},
                new []{'a','a','a','a','a'},
                new []{'a','a','a','a','a'},
            });
        }

        [TestMethod]
        public void Center()
        {
            Assert(VerticalAlignment.Center, new []
            {
                new []{'a','a','a','a','a'},
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'-','-','-','-','-'},
                new []{'a','a','a','a','a'}
            });
        }

        [TestMethod]
        public void Bottom()
        {
            Assert(VerticalAlignment.Bottom, new []
            {
                new []{'a','a','a','a','a'},
                new []{'a','a','a','a','a'},
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'-','-','-','-','-'},
            });
        }

        [TestMethod]
        public void Stretch()
        {
            Assert(VerticalAlignment.Stretch, new []
            {
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'-','-','-','-','-'}
            });
        }

        [TestMethod]
        public void Unknown()
        {
            Assert(VerticalAlignment.Unknown, new []
            {
                new []{'-','-','-','-','-'},
                new []{'|','z',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'|',' ',' ',' ','|'},
                new []{'-','-','-','-','-'}
            });
        }

        public void Assert (VerticalAlignment verticalAlignment, char[][] expected)
        {
            var main = new StackPanel()
            {
                Size = new Size(5, 5)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));

            var modal = new Buffer
            {
                Border            = new Thickness(1),
                VerticalAlignment = verticalAlignment,
                Size              = new Size(3, 3)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(expected, Render(main.Size, main));
        }
    }

    [TestClass]
    public class StackPanelCentered : TestBase
    {
        [TestMethod]
        public void CanCenterHorizontallyAndVertically ()
        {
            var main = new StackPanel()
            {
                Size = new Size(5, 5)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Size                = new Size(3, 3)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'a','a','a','a','a'},
                new []{'a','-','-','-','a'},
                new []{'a','|','z','|','a'},
                new []{'a','-','-','-','a'},
                new []{'a','a','a','a','a'}
            }, Render(main.Size, main));
        }
    }

    [TestClass]
    public class StackPanelCenteredHorzontally : TestBase
    {
        [TestMethod]
        public void WhenContentCannotBeCentered ()
        {
            var main = new StackPanel()
            {
                Size = new Size(4, 4)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(3, 3)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-'},
                new []{'|','z',' ','|'},
                new []{'|',' ',' ','|'},
                new []{'-','-','-','-'}
            }, Render(main.Size, main));
        }

        [TestMethod]
        public void WhenContentAndPanelAreTheSameSize ()
        {
            var main = new StackPanel()
            {
                Size = new Size(4, 4)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(4, 4)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-'},
                new []{'|','z',' ','|'},
                new []{'|',' ',' ','|'},
                new []{'-','-','-','-'}
            }, Render(main.Size, main));
        }

        [TestMethod]
        public void WhenContentIsBiggerThanPanel()
        {
            var main = new StackPanel()
            {
                Size = new Size(4, 4)
            };

            main.Add(new Buffer().Wrap(new string('a', 25)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(5, 5)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'-','-','-','-'},
                new []{'|','z',' ','|'},
                new []{'|',' ',' ','|'},
                new []{'-','-','-','-'}
            }, Render(main.Size, main));
        }

        [TestMethod]
        public void WhenPanelWidthIsOddAndContentIsOdd()
        {
            var main = new StackPanel()
            {
                Size = new Size(7, 7)
            };

            main.Add(new Buffer().Wrap(new string('a', 49)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(3, 3)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'a','a','-','-','-','a','a'},
                new []{'a','a','|','z','|','a','a'},
                new []{'a','a','|',' ','|','a','a'},
                new []{'a','a','|',' ','|','a','a'},
                new []{'a','a','|',' ','|','a','a'},
                new []{'a','a','|',' ','|','a','a'},
                new []{'a','a','-','-','-','a','a'},
            }, Render(main.Size, main));
        }

        [TestMethod]
        public void WhenPanelWidthIsOddAndContentIsEven()
        {
            var main = new StackPanel()
            {
                Size = new Size(7, 7)
            };

            main.Add(new Buffer().Wrap(new string('a', 49)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(4, 4)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'a','-','-','-','-','-','a'},
                new []{'a','|','z',' ',' ','|','a'},
                new []{'a','|',' ',' ',' ','|','a'},
                new []{'a','|',' ',' ',' ','|','a'},
                new []{'a','|',' ',' ',' ','|','a'},
                new []{'a','|',' ',' ',' ','|','a'},
                new []{'a','-','-','-','-','-','a'},
            }, Render(main.Size, main));
        }

        [TestMethod]
        public void WhenPanelWidthIsEvenAndContentIsEven()
        {
            var main = new StackPanel()
            {
                Size = new Size(6, 6)
            };

            main.Add(new Buffer().Wrap(new string('a', 49)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(4, 4)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'a','-','-','-','-','a'},
                new []{'a','|','z',' ','|','a'},
                new []{'a','|',' ',' ','|','a'},
                new []{'a','|',' ',' ','|','a'},
                new []{'a','|',' ',' ','|','a'},
                new []{'a','-','-','-','-','a'},
            }, Render(main.Size, main));
        }

        [TestMethod]
        public void WhenPanelWidthIsEvenAndContentIsOdd()
        {
            var main = new StackPanel()
            {
                Size = new Size(6, 6)
            };

            main.Add(new Buffer().Wrap(new string('a', 49)));

            var modal = new Buffer
            {
                Border              = new Thickness(1),

                HorizontalAlignment = HorizontalAlignment.Center,
                Size                = new Size(3, 3)
            }.WriteLine("z");

            main.Add(modal);

            AssertCellsAreEquivelant(new []
            {
                new []{'a','-','-','-','-','a'},
                new []{'a','|','z',' ','|','a'},
                new []{'a','|',' ',' ','|','a'},
                new []{'a','|',' ',' ','|','a'},
                new []{'a','|',' ',' ','|','a'},
                new []{'a','-','-','-','-','a'},
            }, Render(main.Size, main));
        }
    }

    [TestClass]
    public class ContentControlTests: TestBase
    {
        [TestMethod]
        public void RendersContent()
        {
            var buffer = new Buffer();
            var content = new ContentControl
            {
                Content = buffer
            };
            buffer.WriteLine("abc");

            var cells = Render(new Size(5, 5), content);

            AssertCellsAreEquivelant(new []
            {
                new []{'a','b','c',' ',' '},
                new []{' ',' ',' ',' ',' '},
                new []{' ',' ',' ',' ',' '},
                new []{' ',' ',' ',' ',' '},
                new []{' ',' ',' ',' ',' '}
            }, cells);

        }
    }
}
