import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from './services/api';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private apiService = inject(ApiService);

  token = signal<string | null>(null);
  errorMessage = signal<string>('');
  tasks = signal<any[]>([]);

  loginData = { email: '', password: '' };
  newTask = { title: '', category: 'Ogólne', priority: 'Średni' };

  onLogin() {
    this.errorMessage.set('');
    this.apiService.login(this.loginData).subscribe({
      next: (response: any) => {

        if (response && response.id) {
          const userId = response.id.toString();
          this.token.set(userId);
          this.loadTasks();
        } else {
          this.errorMessage.set('Błąd autoryzacji: API nie zwróciło ID użytkownika.');
        }
      },
      error: (err) => {
        console.error('Błąd logowania w API:', err);
        this.errorMessage.set('Błędny email lub hasło. Upewnij się, że API działa!');
      }
    });
  }

  loadTasks() {
    const userId = this.token();
    if (!userId) return;

    const numericUserId = parseInt(userId);

    this.apiService.getTasks(userId).subscribe({
      next: (data: any[]) => {

        const userTasks = data.filter(task => task.userId === numericUserId);
        this.tasks.set(userTasks);
      },
      error: (err) => {
        console.error('Błąd pobierania zadań:', err);
        this.tasks.set([]);
      }
    });
  }

  onAddTask() {
    const userId = this.token();
    if (!userId || !this.newTask.title.trim()) return;

    const priorityMap: { [key: string]: number } = { 'Niski': 0, 'Średni': 1, 'Wysoki': 2 };

    const taskToSend = {
      title: this.newTask.title,
      category: this.newTask.category,
      priority: priorityMap[this.newTask.priority] ?? 1,
      status: 0,
      userId: parseInt(userId)
    };

    this.apiService.addTask(taskToSend, userId).subscribe({
      next: () => {
        this.newTask.title = '';
        this.loadTasks();
      },
      error: (err) => console.error('Błąd dodawania:', err)
    });
  }

  onCompleteTask(id: number) {
    const userId = this.token();
    if (!userId) return;

    const taskToUpdate = this.tasks().find(t => t.id === id);
    if (!taskToUpdate) return;

    const updatedTask = {
      ...taskToUpdate,
      status: 2
    };

    this.apiService.completeTask(id, updatedTask, userId).subscribe({
      next: () => {
        console.log(`Zadanie o ID ${id} zostało pomyślnie zakończone w API!`);
        this.loadTasks();
      },
      error: (err) => {
        console.error('Błąd podczas kończenia zadania:', err);
      }
    });
  }

  onDeleteTask(id: number) {
    const userId = this.token();
    if (!userId) return;

    this.apiService.deleteTask(id, userId).subscribe({
      next: () => this.loadTasks(),
      error: (err) => console.error(err)
    });
  }

  onLogout() {
    this.token.set(null);
    this.tasks.set([]);
    this.loginData = { email: '', password: '' };
  }
}
