using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MyScreenSaver
{
    public partial class ScreenSaver : Window
    {
        private List<TextBlock> _spaceInvaderTextBlocks;

        private Point _mousePosition;
        private bool _isActive;
        private bool _goingDown;
        private bool _tick;

        private const double VerticalAnimationSpeed = 0.5;
        private const double HorizontalAnimationSpeed = 10;

        public ScreenSaver()
        {
            InitializeComponent();
            _spaceInvaderTextBlocks = new List<TextBlock>()
            {
                txtSpaceInvaders1,
                txtSpaceInvaders2,
                txtSpaceInvaders3,
                txtSpaceInvaders4,
                txtSpaceInvaders5
            };

            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromSeconds(0.5), DispatcherPriority.Normal, TimerTick, this.Dispatcher);

            this.Loaded += (s, e) =>
                {
                    warningText.Text = "Beware\nthe\nInvasion";
                    _goingDown = true;
                    MoveRightAnimation();
                };
        }

        #region Event Handlers

        private void TimerTick(object sender, EventArgs e)
        {
            _tick = !_tick;

            txtSpaceInvaders1.Text = _tick ? "FFFFFFFFFFFFFFFF" : "EEEEEEEEEEEEEEEE";
            txtSpaceInvaders2.Text = _tick ? "DDDDDDDDDDDD" : "CCCCCCCCCCCC";
            txtSpaceInvaders3.Text = _tick ? "DDDDDDDDDDDD" : "CCCCCCCCCCCC";
            txtSpaceInvaders4.Text = _tick ? "BBBBBBBBBBB" : "AAAAAAAAAAA";
            txtSpaceInvaders5.Text = _tick ? "BBBBBBBBBBB" : "AAAAAAAAAAA";
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point currentPosition = e.MouseDevice.GetPosition(this);
            if (!_isActive)
            {
                _mousePosition = currentPosition;
                _isActive = true;
            }
            else
            {
                if ((Math.Abs(_mousePosition.X - currentPosition.X) > 10) || (Math.Abs(_mousePosition.Y - currentPosition.Y) > 10))
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion Event Handlers

        #region Helper Methods

        private void MoveRightAnimation()
        {
            HorizontalAnimation(0, SystemParameters.PrimaryScreenWidth - txtSpaceInvaders1.ActualWidth, false);
        }

        private void MoveLeftAnimation()
        {
            HorizontalAnimation(SystemParameters.PrimaryScreenWidth - txtSpaceInvaders1.ActualWidth, 0, true);
        }

        private void HorizontalAnimation(double from, double to, bool right)
        {
            DoubleAnimation animation = new DoubleAnimation(from, to, TimeSpan.FromSeconds(HorizontalAnimationSpeed));
            animation.Completed += (s, e) =>
            {
                CheckUpOrDown();
                if (_goingDown)
                    MoveDownAnimation(right);
                else
                    MoveUpAnimation(right);
            };

            _spaceInvaderTextBlocks.ForEach(b => b.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation));
        }

        private void MoveDownAnimation(bool right)
        {
            VerticalAnimation(false, right);
        }

        private void MoveUpAnimation(bool right)
        {
            VerticalAnimation(true, right);
        }

        private void VerticalAnimation(bool up, bool right)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(VerticalAnimationSpeed));
            animation.Completed += (s, e) =>
            {
                if (right)
                    MoveRightAnimation();
                else
                    MoveLeftAnimation();
            };

            foreach (var block in _spaceInvaderTextBlocks)
            {
                var yProp = (double)block.RenderTransform.GetValue(TranslateTransform.YProperty);
                animation.To = yProp + (up ? -55 : 55);
                block.RenderTransform.BeginAnimation(TranslateTransform.YProperty, animation);
            }
        }

        private void CheckUpOrDown()
        {
            if (txtSpaceInvaders5.RenderTransform.Value.OffsetY >= (SystemParameters.PrimaryScreenHeight - 700))
            {
                _goingDown = false;
            }
            else if (txtSpaceInvaders1.RenderTransform.Value.OffsetY <= (100))
            {
                _goingDown = true;
            }
        }

        #endregion Helper Methods
    }
}