import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7269/api';

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Auth/login`, credentials);
  }

  getTasks(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Tasks/user/${userId}`);
  }

  addTask(task: any, userId: string): Observable<any> {
    const headers = new HttpHeaders().set('X-User-Id', userId);
    return this.http.post(`${this.apiUrl}/Tasks`, task, { headers });
  }

  completeTask(id: number, task: any, userId: string): Observable<any> {
    const headers = new HttpHeaders().set('X-User-Id', userId);
    return this.http.put(`${this.apiUrl}/Tasks/${id}`, task, { headers });
  }

  deleteTask(id: number, userId: string): Observable<any> {
    const headers = new HttpHeaders().set('X-User-Id', userId);
    return this.http.delete(`${this.apiUrl}/Tasks/${id}`, { headers });
  }
}
