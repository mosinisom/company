// wwwroot/script.js
let socket;
const PAGE_SIZE = 10;
let currentPage = 1;
let departments = [];
let employees = [];
let positions = [];

document.addEventListener("DOMContentLoaded", function () {
  initializeWebSocket();
  loadEmployees();
  loadDepartments();
  loadPositions();
});

function initializeWebSocket() {
  socket = new WebSocket("ws://localhost:5000/ws");

  socket.onopen = () => {
    console.log("WebSocket соединение установлено");
  };

  socket.onmessage = (event) => {
    const response = JSON.parse(event.data);
    handleWebSocketResponse(response);
  };

  socket.onclose = () => {
    console.log("WebSocket соединение закрыто, переподключение...");
    setTimeout(initializeWebSocket, 1000);
  };
}

function sendMessage(message) {
  if (socket.readyState === WebSocket.OPEN) {
    console.log('Sending message:', message);
    socket.send(JSON.stringify(message));
  } else {
    console.error("WebSocket не подключен");
  }
}

function handleWebSocketResponse(response) {
  try {
    switch (response.action) {
      case "getEmployees":
        employees = response.data;
        displayEmployees(response.data);
        break;
      case "getDepartments":
        departments = response.data;
        displayDepartments(response.data);
        break;
      case "getProjects":
        displayProjects(response.data);
        break;
      case "createEmployee":
      case "updateEmployee":
      case "deleteEmployee":
        loadEmployees();
        break;
      case "createDepartment":
      case "updateDepartment":
      case "deleteDepartment":
        loadDepartments();
        break;
      case "createProject":
      case "updateProject":
      case "deleteProject":
        loadProjects();
        break;
      case "getPositions":
        positions = response.data;
        displayPositions(response.data);
        break;
      case "createPosition":
      case "updatePosition":
      case "deletePosition":
        loadPositions();
        break;
      case "error":
        showError(response.message);
        break;
    }
  } catch (error) {
    showError('Произошла ошибка при обработке ответа сервера');
    console.error(error);
  }
}

function closeModal() {
  document.querySelector('.modal')?.remove();
}


socket.onerror = function (error) {
  console.error('WebSocket error:', error);
};

function showTab(tabName) {
  document.querySelectorAll('.tab-content').forEach(tab => tab.style.display = 'none');
  document.querySelectorAll('.tab').forEach(tab => tab.classList.remove('active'));
  document.getElementById(tabName).style.display = 'block';
  document.querySelector(`.tab[onclick="showTab('${tabName}')"]`).classList.add('active');

  switch (tabName) {
    case 'employees':
      loadEmployees();
      break;
    case 'departments':
      loadDepartments();
      break;
    case 'projects':
      loadProjects();
      break;
    case 'positions':
      loadPositions();
      break;
  }
}

function displayPositions(positions) {
  const container = document.getElementById('positionsList');
  container.innerHTML = `
    <table class="table">
      <thead>
        <tr>
          <th>Название</th>
          <th>Зарплата</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        ${positions.map(pos => `
          <tr>
            <td>${pos.title}</td>
            <td>${pos.salaryRange}</td>
            <td>
              <button onclick="showPositionForm(${JSON.stringify(pos)})">Редактировать</button>
              <button onclick="deletePosition(${pos.id})">Удалить</button>
            </td>
          </tr>
        `).join('')}
      </tbody>
    </table>
  `;
}

function populatePositionSelect(positions) {
  const select = document.querySelector('select[name="positionId"]');
  select.innerHTML = `
    <option value="">Выберите должность</option>
    ${positions.map(pos => `
      <option value="${pos.Id}">${pos.Title}</option>
    `).join('')}
  `;
}


function showPositionForm(position = null) {
  const modal = document.createElement('div');
  modal.className = 'modal';
  modal.innerHTML = `
    <div class="modal-content">
      <h3>${position ? 'Редактировать' : 'Добавить'} должность</h3>
      <form id="positionForm">
        <div class="form-group">
          <label>Название должности</label>
          <input class="form-control" name="title" value="${position?.title || ''}" required>
        </div>
        <div class="form-group">
          <label>Диапазон зарплаты</label>
          <input class="form-control" name="salaryRange" value="${position?.salaryRange || ''}" required>
        </div>
        <div class="form-group">
          <label>Обязанности</label>
          <textarea class="form-control" name="responsibilities" rows="3">${position?.responsibilities || ''}</textarea>
        </div>
        <button type="submit" class="btn">Сохранить</button>
        <button type="button" class="btn btn-secondary" onclick="closeModal()">Отмена</button>
      </form>
    </div>
  `;

  document.body.appendChild(modal);

  document.getElementById('positionForm').onsubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const data = {
      id: position?.id || 0,
      title: formData.get('title'),
      salaryRange: formData.get('salaryRange'),
      responsibilities: formData.get('responsibilities')
    };

    try {
      sendMessage({
        action: position ? 'updatePosition' : 'createPosition',
        data: data
      });
      closeModal();
    } catch (error) {
      showError(error.message);
    }
  };
}

async function deletePosition(id) {
  if (confirm('Вы уверены, что хотите удалить эту должность?')) {
    try {
      sendMessage({ action: 'deletePosition', id });
    } catch (error) {
      showError(error.message);
    }
  }
}

function loadEmployees() {
  sendMessage({ action: "getEmployees" });
}

function loadDepartments() {
  sendMessage({ action: "getDepartments" });
}

function loadProjects() {
  sendMessage({ action: "getProjects" });
}

function loadPositions() {
  sendMessage({ action: 'getPositions' });
}

function displayEmployees(employees) {
  const container = document.getElementById('employeesList');
  const { items, totalPages } = paginate(employees, currentPage);

  container.innerHTML = `
    <table class="table">
      <thead>
        <tr>
          <th>ФИО</th>
          <th>Email</th>
          <th>Отдел</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        ${items.map(emp => `
          <tr>
            <td>${emp.FirstName} ${emp.LastName}</td>
            <td>${emp.Email}</td>
            <td>${emp.DepartmentName || 'Без отдела'}</td>
            <td>
              <button onclick="showEmployeeForm(${JSON.stringify(emp)})">Редактировать</button>
              <button class="btn-danger" onclick="deleteEmployee(${emp.Id})">Удалить</button>
            </td>
          </tr>
        `).join('')}
      </tbody>
    </table>
  `;

  renderPagination(totalPages, currentPage, 'employeesList');
}


function displayDepartments(departments) {
  const container = document.getElementById('departmentsList');
  container.innerHTML = `
        <table class="table">
            <thead>
                <tr>
                    <th>Название</th>
                    <th>Действия</th>
                </tr>
            </thead>
            <tbody>
                ${departments.map(dep => `
                    <tr>
                        <td>${dep.name}</td>
                        <td>
                            <button onclick="editDepartment(${dep.id})">Редактировать</button>
                            <button onclick="deleteDepartment(${dep.id})">Удалить</button>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;
}

function displayProjects(projects) {
  const container = document.getElementById('projectsList');
  container.innerHTML = `
        <table class="table">
            <thead>
                <tr>
                    <th>Название</th>
                    <th>Отдел</th>
                    <th>Действия</th>
                </tr>
            </thead>
            <tbody>
                ${projects.map(proj => {
            const department = departments.find(dep => dep.id === proj.departmentId);
            return `
                    <tr>
                        <td>${proj.name}</td>
                        <td>${department ? department.name : 'Без отдела'}</td>
                        <td>
                            <button onclick="editProject(${proj.id})">Редактировать</button>
                            <button onclick="deleteProject(${proj.id})">Удалить</button>
                        </td>
                    </tr>
                  `;
              }).join('')}
            </tbody>
        </table>
    `;
}

function showError(message) {
  const alert = document.createElement('div');
  alert.className = 'alert alert-error';
  alert.textContent = message;
  document.querySelector('.container').prepend(alert);
  setTimeout(() => alert.remove(), 3000);
}

function confirm(message) {
  return window.confirm(message);
}

async function showEmployeeForm(employee = null) {
  if (!positions.length) {
    await new Promise(resolve => {
      sendMessage({ action: "getPositions" });
      const handler = (event) => {
        const response = JSON.parse(event.data);
        if (response.action === "getPositions") {
          positions = response.data;
          socket.removeEventListener('message', handler);
          resolve();
        }
      };
      socket.addEventListener('message', handler);
    });
  }

  const modal = document.createElement('div');
  modal.className = 'modal';
  modal.innerHTML = `
    <div class="modal-content">
      <h3>${employee ? 'Редактировать' : 'Добавить'} сотрудника</h3>
      <form id="employeeForm">
        <div class="form-group">
          <label>Имя</label>
          <input class="form-control" name="firstName" value="${employee?.firstName || ''}" required>
        </div>
        <div class="form-group">
          <label>Фамилия</label>
          <input class="form-control" name="lastName" value="${employee?.lastName || ''}" required>
        </div>
        <div class="form-group">
          <label>Email</label>
          <input class="form-control" type="email" name="email" value="${employee?.email || ''}" required>
        </div>
        <div class="form-group">
          <label>Телефон</label>
          <input class="form-control" type="tel" name="phone" value="${employee?.phone || ''}" required>
        </div>
        <div class="form-group">
          <label>Дата рождения</label>
          <input class="form-control" type="date" name="birthDate" value="${employee?.birthDate?.split('T')[0] || ''}" required>
        </div>
        <div class="form-group">
          <label>Дата приема</label>
          <input class="form-control" type="date" name="hireDate" value="${employee?.hireDate?.split('T')[0] || ''}" required>
        </div>
        <div class="form-group">
          <label>Отдел</label>
          <select class="form-control" name="departmentId">
            <option value="">Без отдела</option>
            ${departments?.map(d => `
              <option value="${d.id}" ${employee?.departmentId === d.id ? 'selected' : ''}>
                ${d.name}
              </option>
            `)?.join('') || ''}
          </select>
        </div>
        <div class="form-group">
          <label>Должность</label>
          <select class="form-control" name="positionId" required>
            <option value="">Выберите должность</option>
            ${positions?.map(p => `
              <option value="${p.id}" ${employee?.positionId === p.id ? 'selected' : ''}>
                ${p.title}
              </option>
            `)?.join('') || ''}
          </select>
        </div>
        <div class="form-group">
          <label>Адрес</label>
          <input class="form-control" name="address" value="${employee?.address || ''}" required>
        </div>
        <button type="submit" class="btn">Сохранить</button>
        <button type="button" class="btn btn-secondary" onclick="closeModal()">Отмена</button>
      </form>
    </div>
  `;
  document.body.appendChild(modal);
  document.getElementById('employeeForm').onsubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const data = {
      id: employee?.id || 0,
      firstName: formData.get('firstName'),
      lastName: formData.get('lastName'),
      email: formData.get('email'),
      phone: formData.get('phone'),
      birthDate: formData.get('birthDate'),
      hireDate: formData.get('hireDate'),
      departmentId: formData.get('departmentId') ? parseInt(formData.get('departmentId')) : null,
      positionId: parseInt(formData.get('positionId')),
      address: formData.get('address'),
      createdAt: employee?.createdAt || new Date().toISOString()
    };
    try {
      sendMessage({
        action: employee ? 'updateEmployee' : 'createEmployee',
        data: data
      });
      closeModal();
    } catch (error) {
      showError(error.message);
    }
  };
}

async function deleteEmployee(id) {
  if (confirm('Вы уверены, что хотите удалить этого сотрудника?')) {
    try {
      sendMessage({ action: 'deleteEmployee', id });
    } catch (error) {
      showError(error.message);
    }
  }
}

function paginate(items, page = 1) {
  const start = (page - 1) * PAGE_SIZE;
  const paginatedItems = items.slice(start, start + PAGE_SIZE);
  return {
    items: paginatedItems,
    totalPages: Math.ceil(items.length / PAGE_SIZE)
  };
}

function renderPagination(totalPages, currentPage, containerId) {
  const container = document.getElementById(containerId);
  const pagination = document.createElement('div');
  pagination.className = 'pagination';

  let html = '';
  for (let i = 1; i <= totalPages; i++) {
    html += `<button class="page-btn ${i === currentPage ? 'active' : ''}" 
      onclick="changePage(${i}, '${containerId}')">${i}</button>`;
  }

  pagination.innerHTML = html;
  container.appendChild(pagination);
}



function showDepartmentForm(department = null) {
  const modal = document.createElement('div');
  modal.className = 'modal';
  modal.innerHTML = `
    <div class="modal-content">
      <h3>${department ? 'Редактировать' : 'Добавить'} отдел</h3>
      <form id="departmentForm">
        <div class="form-group">
          <label>Название</label>
          <input class="form-control" name="name" value="${department?.name || ''}" required>
        </div>
        <div class="form-group">
          <label>Руководитель</label>
          <select class="form-control" name="managerId">
            <option value="">Без руководителя</option>
            ${employees?.map(e => `
              <option value="${e.id}" ${department?.managerId === e.id ? 'selected' : ''}>
                ${e.firstName} ${e.lastName}
              </option>
            `)?.join('') || ''}
          </select>
        </div>
        <button type="submit" class="btn">Сохранить</button>
        <button type="button" class="btn btn-secondary" onclick="closeModal()">Отмена</button>
      </form>
    </div>
  `;

  document.body.appendChild(modal);
  document.getElementById('departmentForm').onsubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const managerId = formData.get('managerId');
    const name = formData.get('name')?.trim();

    if (!name) {
      showError('Название отдела обязательно для заполнения');
      return;
    }

    const data = {
      id: department?.id || 0,
      name: name,
      managerId: managerId ? parseInt(managerId) : null,
      createdAt: department?.createdAt || new Date().toISOString()
    };

    try {
      sendMessage({
        action: department ? 'updateDepartment' : 'createDepartment',
        data: data
      });
      closeModal();
    } catch (error) {
      showError(error.message);
    }
  };
}

async function deleteDepartment(id) {
  if (confirm('Вы уверены, что хотите удалить этот отдел?')) {
    try {
      sendMessage({ action: 'deleteDepartment', id });
    } catch (error) {
      showError(error.message);
    }
  }
}

function showProjectForm(project = null) {
  const modal = document.createElement('div');
  modal.className = 'modal';
  modal.innerHTML = `
    <div class="modal-content">
      <h3>${project ? 'Редактировать' : 'Добавить'} проект</h3>
      <form id="projectForm">
        <div class="form-group">
          <label>Название</label>
          <input class="form-control" name="name" value="${project?.name || ''}" required>
        </div>
        <div class="form-group">
          <label>Описание</label>
          <textarea class="form-control" name="description" rows="3">${project?.description || ''}</textarea>
        </div>
        <div class="form-group">
          <label>Статус</label>
          <select class="form-control" name="status" required>
            <option value="new" ${project?.status === 'new' ? 'selected' : ''}>Новый</option>
            <option value="in_progress" ${project?.status === 'in_progress' ? 'selected' : ''}>В работе</option>
            <option value="completed" ${project?.status === 'completed' ? 'selected' : ''}>Завершен</option>
          </select>
        </div>
        <div class="form-group">
          <label>Дата начала</label>
          <input type="date" class="form-control" name="startDate" 
            value="${project?.startDate?.split('T')[0] || ''}" required>
        </div>
        <div class="form-group">
          <label>Дата окончания</label>
          <input type="date" class="form-control" name="endDate" 
            value="${project?.endDate?.split('T')[0] || ''}">
        </div>
        <button type="submit" class="btn">Сохранить</button>
        <button type="button" class="btn btn-secondary" onclick="closeModal()">Отмена</button>
      </form>
    </div>
  `;

  document.body.appendChild(modal);

  document.getElementById('projectForm').onsubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const data = {
      id: project?.id,
      name: formData.get('name'),
      description: formData.get('description'),
      status: formData.get('status'),
      startDate: formData.get('startDate'),
      endDate: formData.get('endDate') || null,
      createdAt: new Date().toISOString()
    };

    try {
      sendMessage({
        action: project ? 'updateProject' : 'createProject',
        data: data
      });
      closeModal();
    } catch (error) {
      showError(error.message);
    }
  };
}

async function deleteProject(id) {
  if (confirm('Вы уверены, что хотите удалить этот проект?')) {
    try {
      sendMessage({ action: 'deleteProject', id });
    } catch (error) {
      showError(error.message);
    }
  }
}

