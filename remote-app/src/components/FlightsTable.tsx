import React, { useEffect, useState } from "react";
import axios from "axios";

interface Flight {
    id: number;
    number: string;
    departureCity: string;
    arrivalCity: string;
    status: string;
    appUser: { firstName: string; lastName: string };
}

const FlightsTable: React.FC = () => {
    const [flights, setFlights] = useState<Flight[]>([]);

    useEffect(() => {
        axios.get<Flight[]>("http://localhost:5263/api/flights")
            .then(res => setFlights(res.data))
            .catch(err => console.error(err));
    }, []);

    return (
        <table>
            <thead>
                <tr>
                    <th>Number</th>
                    <th>Departure</th>
                    <th>Arrival</th>
                    <th>Status</th>
                    <th>User</th>
                </tr>
            </thead>
            <tbody>
                {flights.map(f => (
                    <tr key={f.id}>
                        <td>{f.number}</td>
                        <td>{f.departureCity}</td>
                        <td>{f.arrivalCity}</td>
                        <td>{f.status}</td>
                        <td>{f.appUser.firstName} {f.appUser.lastName}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
};

export default FlightsTable;