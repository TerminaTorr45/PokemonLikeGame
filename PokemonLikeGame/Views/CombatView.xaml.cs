using ExerciceMonster.Models;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ExerciceMonster.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace ExerciceMonster.Views
{
    public partial class CombatView : UserControl, INotifyPropertyChanged
    {
        private readonly string _connectionString;
        private Monster _playerMonster;
        private Monster _enemyMonster;
        private ObservableCollection<Spell> _playerSpells = new ObservableCollection<Spell>();
        private int _playerHP;
        private int _enemyHP;

        public event PropertyChangedEventHandler PropertyChanged;

        public CombatView()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ExerciceMonsterContext"].ConnectionString;

            InitializeComponent();
            DataContext = this;
            LoadPlayerMonster();
            LoadEnemyMonster();
            AttackCommand = new RelayCommand(Attack, CanAttack);
            RestartCombatCommand = new RelayCommand(RestartCombat);
     
        }

        public Monster PlayerMonster
        {
            get => _playerMonster;
            set
            {
                _playerMonster = value;
                OnPropertyChanged();
            }
        }

        public Monster EnemyMonster
        {
            get => _enemyMonster;
            set
            {
                _enemyMonster = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Spell> PlayerSpells
        {
            get => _playerSpells;
            set
            {
                _playerSpells = value;
                OnPropertyChanged();
            }
        }

        public int PlayerHP
        {
            get => _playerHP;
            set
            {
                _playerHP = value;
                OnPropertyChanged();
            }
        }

        public int EnemyHP
        {
            get => _enemyHP;
            set
            {
                _enemyHP = value;
                OnPropertyChanged();
            }
        }

        public ICommand AttackCommand { get; private set; }
        public ICommand RestartCombatCommand { get; private set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanAttack(object parameter)
        {
            return EnemyHP > 0 && PlayerHP > 0;
        }

        private void Attack(object parameter)
        {
            if (parameter is Spell spell)
            {
                // Player attacks enemy
                EnemyHP -= spell.Damage;
                if (EnemyHP <= 0)
                {
                    EnemyHP = 0;
                    MessageBox.Show("You have defeated the enemy monster!", "Victory", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Enemy attacks back
                EnemyAttack();
            }
        }

        private void EnemyAttack()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Fetch a random spell for the enemy monster
                var query = "SELECT TOP 1 s.Damage FROM MonsterSpell ms JOIN Spell s ON ms.SpellID = s.ID WHERE ms.MonsterID = @MonsterID ORDER BY NEWID()";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MonsterID", EnemyMonster.ID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PlayerHP -= reader.GetInt32(0);
                            if (PlayerHP <= 0)
                            {
                                PlayerHP = 0;
                                MessageBox.Show("Your monster has been defeated!", "Defeat", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
        }

        private void RestartCombat(object parameter)
        {
            LoadEnemyMonster();
            EnemyHP = EnemyMonster.Health;
            PlayerHP = PlayerMonster.Health;
        }

        private void LoadPlayerMonster()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Query to fetch the first player's monster
                var query = @"
                    SELECT TOP 1 m.ID, m.Name, m.Health, m.ImageURL
                    FROM PlayerMonster pm
                    JOIN Monster m ON pm.MonsterID = m.ID";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        PlayerMonster = new Monster
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Health = reader.GetInt32(2),
                            ImageURL = reader.IsDBNull(3) ? null : reader.GetString(3)
                        };

                        PlayerHP = PlayerMonster.Health;
                    }
                }

                // Fetch spells for the player's monster
                query = "SELECT s.ID, s.Name, s.Damage FROM MonsterSpell ms JOIN Spell s ON ms.SpellID = s.ID WHERE ms.MonsterID = @MonsterID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MonsterID", PlayerMonster.ID);

                    using (var reader = command.ExecuteReader())
                    {
                        PlayerSpells.Clear();
                        while (reader.Read())
                        {
                            PlayerSpells.Add(new Spell
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Damage = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
        }

        private void LoadEnemyMonster()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"
                    SELECT TOP 1 ID, Name, Health, ImageURL
                    FROM Monster
                    ORDER BY NEWID()";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        EnemyMonster = new Monster
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Health = (int)(reader.GetInt32(2) * 1.10), // +10% HP
                            ImageURL = reader.IsDBNull(3) ? null : reader.GetString(3)
                        };

                        EnemyHP = EnemyMonster.Health;
                    }
                }
            }
        }

    }
}
